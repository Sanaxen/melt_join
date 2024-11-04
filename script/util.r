library(data.table)
library(dplyr)
library(lubridate)
library(reshape2)
library(Matrix)
library(xgboost)
library(tidyverse)
library(scales)
library(skimr)
library(tibble)
library(tidyr) 
library(stringr)  
library(ggthemes)
library(ggpubr)
library(timetk)
library(magrittr)
library(tidymodels)
library(ggplot2)
library(plotly)
library(patchwork)
library(htmlwidgets)

find_closest_factors <- function(n) {
  sqrt_n <- floor(sqrt(n))
  for (i in sqrt_n:1) {
    if (n %% i == 0) {
      return(c(i, n/i))
    }
  }
}

plot_line_df <- function(df, x)
{
	df <- as.data.frame(df)

	library(cowplot)
	
	if ( nrow(df) > 10000 )
	{
		df <- sample_n(tbl = df, size = 10000)
	}
	
	numeric_columns <- select_if(df, is.numeric)
	name <- colnames(numeric_columns)
	if ( length(name) > 25 )
	{
		name <- sample(name, size = 25)
	}

	pltlist=NULL
	for ( i in 1:length(name))
	{
		plt <- numeric_columns[,name[i]] %>% as.data.frame() %>% ggplot() +
		geom_line(aes(x=df[,x], y = .),colour = "dodgerblue4", linewidth=1.0)+
		labs(x = name[i])
		
		pltlist <- c(pltlist, list(plt))
	}
	n = as.integer(sqrt(length(pltlist)))
	plt <- plot_grid(plotlist = pltlist, ncol = n, nrow=n)
	plt
	ggsave(filename="line.png", plt, limitsize=F, width = 16, height = 9)
	
	ggpltlist=list()
	for ( i in 1:length(pltlist))
	{
		ggpltlist[[i]] <- pltlist[[i]]
	}
	
	n = find_closest_factors(length(ggpltlist))
	if ( n[1] == 1 && length(ggpltlist) > 1) n = find_closest_factors(length(ggpltlist)+1)
	gg_plotly <- plotly::subplot(ggpltlist, nrows = n[2])

	print(gg_plotly)
	htmlwidgets::saveWidget(as_widget(gg_plotly), "line.html", selfcontained = F)
	
	return(plt)
}

plot_hist_df <- function(df)
{
	df <- as.data.frame(df)

	library(cowplot)
	numeric_columns <- select_if(df, is.numeric)
	name <- colnames(numeric_columns)
	if ( length(name) > 25 )
	{
		name <- sample(name, size = 25)
	}
	
	pltlist=NULL
	for ( i in 1:length(name))
	{
		plt <- numeric_columns[,name[i]] %>% as.data.frame() %>% ggplot() +
		geom_histogram(aes(x = .),colour = "gray10", fill = "dodgerblue4")+
		labs(x = name[i])
		
		pltlist <- c(pltlist, list(plt))
	}
	n = as.integer(sqrt(length(pltlist)))
	plt <- plot_grid(plotlist = pltlist, ncol = n, nrow=n)
	plt
	ggsave(filename="hist.png", plt, limitsize=F, width = 16, height = 9)
	
	ggpltlist=list()
	for ( i in 1:length(pltlist))
	{
		ggpltlist[[i]] <- pltlist[[i]]
	}
	
	n = find_closest_factors(length(ggpltlist))
	if ( n[1] == 1 && length(ggpltlist) > 1) n = find_closest_factors(length(ggpltlist)+1)
	gg_plotly <- plotly::subplot(ggpltlist, nrows = n[2])

	print(gg_plotly)
	htmlwidgets::saveWidget(as_widget(gg_plotly), "hist.html", selfcontained = F)

	return(plt)
}

plot_predict1 <- function( x, y, id, train, valid, predict, timeUnit="week")
{

	line_color_bule ="#00AFC5"
	line_color_red ="#FF7042"

	library(cowplot)
	timestep=timeUnit
	t <- train

	if ( !is.null(valid) && nrow(valid)> 0 )
	{
		t <- bind_rows(train, valid)
	}
	if ( nrow(t) > 5*nrow(predict) )
	{
		t <- t[(nrow(t)-(5*nrow(predict))):nrow(t),]
	}

	#t <- df
	predict_df <- t %>% full_join(predict) %>% as.data.frame()
	
	if ( id != "" )
	{
		tmp <- predict_df %>% rename("id" = id)
	}else
	{
		tmp <- predict_df
	}
	tmp <- tmp %>% rename("date" = x)
	tmp <- tmp %>% rename("target" = y)
	tmp$date <- as.POSIXct(tmp$date, tz='UTC')


	tmp2 <- tmp
	nn <- 1
	IDs = NULL
	if ( id != "" )
	{
		IDs = unique(tmp$id)
		nn <- length(IDs)
		if ( length(IDs) > 12 )
		{
			IDs <- sample(IDs, size = 12)
		}
		IDs <- as.vector(IDs)
		tmp2 <- tmp %>% filter(IDs[1] == id)
		for ( i in 2:length(IDs))
		{
			tmp2 <- dplyr::bind_rows(tmp2, tmp %>% filter(IDs[i] == id))
		}	
	}

	tmp <- tmp2
	
	if ( id != "" )
	{
		plt <- tmp %>% 
		  ggplot(aes(x = date, y = target, color=id))+
		  geom_line(linewidth =0.5,linetype = "dotted")+
		  geom_line(aes(x = date, y = predict, color = id),linewidth =0.6)+
		  geom_ribbon(aes(ymin = lower, ymax = upper), alpha = .3)
		  #+
		  #scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
		  #theme(axis.text.x = element_text(angle = 90, hjust = 1))
	}else
	{
		plt <- tmp %>% 
		  ggplot(aes(x = date, y = target))+
		  geom_line(linewidth =0.5, color = line_color_bule, linetype = "dotted")+
		  geom_line(aes(x = date, y = predict),linewidth =1.2, color = line_color_red)+
		  geom_ribbon(aes(ymin = lower, ymax = upper), alpha = .3)
		  #+
		  #scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
		  #theme(axis.text.x = element_text(angle = 90, hjust = 1))
	}
	n <- length(unique(tmp$id))
	if ( n > 25 )
	{
		#plt <- plt +  theme(legend.position = 'none')
	}
	
	plt
	ggsave(filename="predict1.png", plt, limitsize=F, width = 16, height = 9)
	
	plt_plotly <- ggplotly(plt)
	print(plt_plotly)
	htmlwidgets::saveWidget(as_widget(plt_plotly), "predict1.html", selfcontained = F)

	rm(tmp)
	return(plt)
	  
}

plot_predict2 <- function( x, y, id, train, valid, predict, timeUnit="week")
{

	library(cowplot)
	timestep=timeUnit
	t <- train

	if ( !is.null(valid)  && nrow(valid)> 0)
	{
		t <- bind_rows(train, valid)
	}
	if ( nrow(t) > 5*nrow(predict) )
	{
		#t <- t[(nrow(t)-(5*nrow(predict))):nrow(t),]
	}

	t2 <- t %>% rename("target" = y)
	t$upper <- t2$target
	t$lower <- t2$target
	t$predict <- t2$target
	
	#t <- df
	predict_df <- t %>% full_join(predict) %>% as.data.frame()

	if ( id != "" )
	{
		tmp <- predict_df %>% rename("id" = id)
	}else
	{
		tmp <- predict_df
	}
	tmp <- tmp %>% rename("date" = x)
	tmp <- tmp %>% rename("target" = y)

	line_color_bule ="#00AFC5"
	line_color_red ="#FF7042"

	nn <- 1
	IDs = NULL
	if ( id != "" )
	{
		IDs = unique(tmp$id)
		nn <- length(IDs)
		if ( length(IDs) > 25 )
		{
			IDs <- sample(IDs, size = 25)
		}
		IDs <- as.vector(IDs)
	}
	
	pltlist=NULL

	if ( !is.null(IDs))
	{
		n <- length(IDs)
	}else
	{
		n <- 1
	}
	for ( i in 1:n)
	{
		if ( is.null(IDs))
		{
			tmp2 <- tmp
		}else
		{
			tmp2 <- tmp %>% filter(id == IDs[i])
		}
		if ( nrow(tmp2) > 5*nrow(predict)/nn )
		{
			tmp2 <- tmp2[(nrow(tmp2)-(5*nrow(predict)/nn)):nrow(tmp2),]
		}
		
		plt <- tmp2 %>%
		  ggplot() +
		  geom_line( aes(x = date, y = predict), color = line_color_red, linewidth =0.6) +
		  geom_line( aes(x = date, y = target), color = line_color_bule, linewidth =0.6) +
  		  geom_ribbon(aes(x = date, y = predict, ymin = lower, ymax = upper), alpha = .3)

		
		if (F)
		{
		plt <- tmp2 %>%
		  ggplot() +
		  geom_line( aes(x = date, y = predict), color = line_color_red, linewidth =0.6) +
		  geom_line( aes(x = date, y = target), color = line_color_bule, linewidth =0.6) +
		  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
		  theme(axis.text.x = element_text(angle = 90, hjust = 1))
		}
		if ( !is.null(IDs))
		{
			plt <- plt  + labs(x = IDs[i])
		}else
		{
			plt <- plt
		}
		
		pltlist <- c(pltlist, list(plt))
	}
	plt <- plot_grid(plotlist = pltlist)

	plt
	ggsave(filename="predict2.png", plt, limitsize=F, width = 16, height = 9)
	
	ggpltlist=list()
	for ( i in 1:length(pltlist))
	{
		ggpltlist[[i]] <- pltlist[[i]]
	}
	
	n = find_closest_factors(length(ggpltlist))
	if ( n[1] == 1 && length(ggpltlist) > 1) n = find_closest_factors(length(ggpltlist)+1)
	gg_plotly <- plotly::subplot(ggpltlist, nrows = n[2])

	print(gg_plotly)
	htmlwidgets::saveWidget(as_widget(gg_plotly), "predict2.html", selfcontained = F)

	rm(tmp)
	return(plt)
	
}


#ids_cols:en:Column name string to be fixed (jp:ŒÅ’è‚·‚é—ñ–¼•¶Žš—ñ) example: c("date_time", "deg_C", "relative_humidity")
#key_cols:en:Column name strings lined up for each column you want to arrange vertically (jp:c‚É•À‚×‚½‚¢—ñ–ˆ‚É•À‚ñ‚Å‚¢‚é—ñ–¼•¶Žš—ñ) example:c("target_carbon_monoxide", "target_benzene","target_nitrogen_oxides")
horizontally_to_vertically <- function(df, ids_cols, key_cols)
{
	df2 <- reshape2::melt(df, id.vars=ids_cols, measure.vars=key_cols, 
				variable.name="key",value.name="target")
	
	return (df2)
}

#ids_cols:en:Column name string to be fixed (jp:ŒÅ’è‚·‚é—ñ–¼•¶Žš—ñ) example: c("date_time", "deg_C", "relative_humidity")
#key: en:Column name string you want to lay down (jp:‰¡‚É‚µ‚½‚¢—ñ–¼•¶Žš—ñ)
vertically_to_horizontally <- function(df, ids_cols, key="key")
{
	fomuler = ids_cols[1]
	for ( i in 2:length(ids_cols) )
	{
		fomuler = paste(fomuler, "+", ids_cols[i],sep="")
	}
	fomuler = paste(fomuler, "~ key",sep="")
	df3<-reshape2::dcast(df2, eval(parse(text =fomuler)) , value.var="target")
	
	return (df3)
}


predict_measure <- function(predict, x="", y= "", id="")
{
#x='date'
#y='sale'
#id ='id'
#timeUnit='day'
	if ( id != "" )
	{
		tmp <- predict %>% rename("id" = id)
	}else
	{
		tmp <- predict
	}
	if ( x != "" )
	{
		tmp <- tmp %>% rename("date" = x)
	}
	
	tmp <- tmp %>% rename("target" = y)

	if (id == "" || x == "")
	{
		summary1 <- tmp %>%  
		summarise(count = n(),
	             MSE=sum(target-predict)^2/count,
	             RMSE=sqrt(sum(target-predict)^2/count),
	             MAE =sum(abs(target-predict))/count,
	             MER =median(abs(target-predict)/target),
	             MAPE=100*sum(abs(target-predict)/target)/count,
	             MSEL=sum((log(1+target)-log(1+predict))^2)/count,
	             RMSEL=sqrt(sum((log(1+target)-log(1+predict))^2)/count))
	    print(summary1)
		meas_plt <- gridExtra::tableGrob(summary1)
		plot(meas_plt)
    	ggsave(filename="predict_measure.png", meas_plt, limitsize=F, width = 16, height = 9)

	    return(summary1)
	}else
	{
		summary2 <- tmp %>%  group_by(id) %>%
		summarise(count = n(),
	             MSE=sum(target-predict)^2/count,
	             RMSE=sqrt(sum(target-predict)^2/count),
	             MAE =sum(abs(target-predict))/count,
	             MER =median(abs(target-predict)/target),
	             MAPE=100*sum(abs(target-predict)/target)/count,
	             MSEL=sum((log(1+target)-log(1+predict))^2)/count,
	             RMSEL=sqrt(sum((log(1+target)-log(1+predict))^2)/count))
	    sink(file = "summary2.txt")
    	print(summary2, n=unique(id))
    	sink()

		IDs = NULL
		if ( id != "" )
		{
			IDs = unique(tmp$id)
			if ( length(IDs) > 12 )
			{
				IDs <- sample(IDs, size = 12)
			}
		
			IDs <- as.vector(IDs)
			tmp2 <- tmp %>% filter(IDs[1] == id)
			for ( i in 2:length(IDs))
			{
				tmp2 <- dplyr::bind_rows(tmp2, tmp %>% filter(IDs[i] == id))
			}	
			tmp <- tmp2
		}
		
		summary2 <- tmp %>% filter(id==IDs) %>% group_by(id) %>%
		summarise(count = n(),
	             MSE=sum(target-predict)^2/count,
	             RMSE=sqrt(sum(target-predict)^2/count),
	             MAE =sum(abs(target-predict))/count,
	             MER =median(abs(target-predict)/target),
	             MAPE=100*sum(abs(target-predict)/target)/count,
	             MSEL=sum((log(1+target)-log(1+predict))^2)/count,
	             RMSEL=sqrt(sum((log(1+target)-log(1+predict))^2)/count))

		meas_plt <- gridExtra::tableGrob(summary2)
		plot(meas_plt)
    	ggsave(filename="predict_measure.png", meas_plt, limitsize=F, width = 16, height = 0.7*length(unique(IDs)))
	    return(summary2)
	}
    return(NULL)
}