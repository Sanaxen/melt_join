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

plot_line_df <- function(df, x)
{
	df <- as.data.frame(df)

	library(cowplot)
	
	df <- sample_n(tbl = df, size = 10000)
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
		geom_line(aes(x=df[,x], y = .),colour = "dodgerblue4", linewidth=0.1)+
		labs(x = name[i])
		
		pltlist <- c(pltlist, list(plt))
	}
	n = as.integer(sqrt(length(pltlist)))
	plt <- plot_grid(plotlist = pltlist, ncol = n, nrow=n)
	plt
	ggsave(filename="line.png", plt, limitsize=F, width = 16, height = 9)
	
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
	
	return(plt)
}

plot_data <- function( x, y, id, train, valid, predict)
{
	t <- train

	if ( !is.null(valid) )
	{
		t <- bind_rows(train, valid)
	}
	if ( nrow(t) > 5*nrow(predict) )
	{
		t <- t[(nrow(t)-(5*nrow(predict))):nrow(t),]
	}

	#t <- df
	predict_df <- t %>% full_join(predict) %>% as.data.frame()

	line_color_bule ="#00AFC5"
	line_color_red ="#FF7042"
	timestep="year"
	plt <- predict_df %>% 
	  ggplot(aes(x = predict_df[,x], y = predict_df[,y])) +
	  geom_line(color = line_color_bule) +
	  geom_line(aes(y = predict), color = line_color_red) +
	  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
	  theme(axis.text.x = element_text(angle = 90, hjust = 1)) +
	  facet_wrap(~predict_df[,id] )

	plt

	s = unique(predict_df[,id])
	if ( length(s) > 25 )
	{
		s <- sample(s, size = 25)
	}
	s <- as.vector(s)



	line_color_bule ="#00AFC5"
	line_color_red ="#FF7042"
	timestep="year"
	plt <- df %>% filter(!!sym(id) %in% !!sym(s)) %>%
	  ggplot(aes(x = predict_df[,x], y = predict_df[,y])) +
	  geom_line(color = line_color_bule) +
	  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
	  theme(axis.text.x = element_text(angle = 90, hjust = 1)) +
	  facet_wrap(~predict_df[,id] )

	plt
	
	plt <- df %>% group_by(id) %>%
	  ggplot(aes(x = predict_df[,x], y = predict_df[,y])) +
	  geom_line(color = line_color_bule) +
	  scale_x_datetime(breaks = date_breaks(timestep), labels = date_format("%Y-%m-%d %H")) +
	  theme(axis.text.x = element_text(angle = 90, hjust = 1)) +
	  facet_wrap(~predict_df[,id] )

	plt
	
}


#ids_cols:en:Column name string to be fixed (jp:å≈íËÇ∑ÇÈóÒñºï∂éöóÒ) example: c("date_time", "deg_C", "relative_humidity")
#key_cols:en:Column name strings lined up for each column you want to arrange vertically (jp:ècÇ…ï¿Ç◊ÇΩÇ¢óÒñàÇ…ï¿ÇÒÇ≈Ç¢ÇÈóÒñºï∂éöóÒ) example:c("target_carbon_monoxide", "target_benzene","target_nitrogen_oxides")
horizontally_to_vertically <- function(df, ids_cols, key_cols)
{
	df2 <- reshape2::melt(df, id.vars=ids_cols, measure.vars=key_cols, 
				variable.name="key",value.name="target")
	
	return (df2)
}

#ids_cols:en:Column name string to be fixed (jp:å≈íËÇ∑ÇÈóÒñºï∂éöóÒ) example: c("date_time", "deg_C", "relative_humidity")
#key: en:Column name string you want to lay down (jp:â°Ç…ÇµÇΩÇ¢óÒñºï∂éöóÒ)
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


predict_measure <- function(pred, target)
{
	summary1 <- pred %>%  
	summarise(count = n(),
             MSE=sum(target-.pred)^2/count,
             RMSE=sqrt(sum(target-.pred)^2/count),
             MAE =sum(abs(target-.pred))/count,
             MER =median(abs(target-.pred)/target),
             MAPE=100*sum(abs(target-.pred)/target)/count,
             MSEL=sum((log(1+target)-log(1+.pred))^2)/count,
             RMSEL=sqrt(sum((log(1+target)-log(1+.pred))^2)/count))

	summary2 <- pred %>%  group_by(date,key) %>%
	summarise(count = n(),
             MSE=sum(target-.pred)^2/count,
             RMSE=sqrt(sum(target-.pred)^2/count),
             MAE =sum(abs(target-.pred))/count,
             MER =median(abs(target-.pred)/target),
             MAPE=100*sum(abs(target-.pred)/target)/count,
             MSEL=sum((log(1+target)-log(1+.pred))^2)/count,
             RMSEL=sqrt(sum((log(1+target)-log(1+.pred))^2)/count))
             
    print(summary1)
    print(summary2)
    return(list(summary1,summary2))
}