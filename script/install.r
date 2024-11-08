curdir <- getwd()
print(curdir)

install_libpath <- paste(curdir, "/lib", sep="")
print(install_libpath)

.libPaths(c(install_libpath,.libPaths()))
print(.libPaths())

install.packages("data.table", repo="http://cran.r-project.org", lib=install_libpath) 

install.packages("sqldf", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("splitstackshape", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("skimr", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("timetk", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("ggthemes", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("remotes", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("rlang", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("lubridate", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("reshape2", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("tidyverse", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("tidyr", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("ggpubr", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("timetk", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("RcppRoll", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("rlang", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("xgboost", repos = "http://cran.us.r-project.org",dependencies=TRUE, lib=install_libpath)
install.packages("dplyr", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("ggplot2", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("plotly", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("htmlwidgets", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("lightgdm", repo="http://cran.r-project.org", lib=install_libpath) 
install.packages("Ckmeans.1d.dp", repo="http://cran.r-project.org", lib=install_libpath) 

quit()
if(!require(devtools)) install.packages("devtools",repo="http://cran.r-project.org", lib=install_libpath)
devtools::install_github("kassambara/ggpubr", lib=install_libpath)


PKG_URL <- "https://github.com/microsoft/LightGBM/releases/download/v4.4.0/lightgbm-4.4.0-r-cran.tar.gz"
remotes::install_url(PKG_URL)

install.packages("cli")
#update.packages(ask = FALSE)