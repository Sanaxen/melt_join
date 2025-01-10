curdir <- getwd()
print(curdir)

install_libpath <- paste(curdir, "/lib", sep="")
print(install_libpath)

.libPaths(c(install_libpath,.libPaths()))
print(.libPaths())

install.packages("data.table", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 

install.packages("sqldf", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("splitstackshape", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("skimr", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("timetk", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("ggthemes", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("remotes", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("rlang", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("lubridate", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("reshape2", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("tidyverse", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("tidyr", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("ggpubr", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("timetk", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("RcppRoll", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("rlang", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("xgboost", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath, type = "binary")
install.packages("dplyr", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("ggplot2", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("plotly", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("htmlwidgets", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("lightgdm", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 
install.packages("Ckmeans.1d.dp", repo="http://cran.r-project.org", lib=install_libpath, type = "binary") 

quit()
if(!require(devtools)) install.packages("devtools",repo="http://cran.r-project.org", lib=install_libpath, type = "binary")
devtools::install_github("kassambara/ggpubr", lib=install_libpath, type = "binary")


PKG_URL <- "https://github.com/microsoft/LightGBM/releases/download/v4.4.0/lightgbm-4.4.0-r-cran.tar.gz"
remotes::install_url(PKG_URL)

install.packages("cli")

#install.packages("tsibble")
#install.packages("fable")

#update.packages(ask = FALSE)