# R语言记录 #
----------
http://blog.csdn.net/jack237/article/details/8210598
## 对象，模式和属性 ##
**对象**是R所进行操作的实体，对象可以是向量、列表等，详见1.6.
**对象的模式**包括numeri，ccomplex，character，logical，list，function，expression等。可以用mode(object)查看。
**对象的长度**是对象的另一固有属性。可以用length(object)查看。
attr()，attributes()： 对象 属性 
mode()，typeof()：对象存储模式与类型 
names()：对象的名字属性
class():对象的class属性
```R
#attributes函数
> attributes(m_time_2)
$names
 [1] "sec"    "min"    "hour"   "mday"   "mon"    "year"   "wday"   "yday"   "isdst" 
[10] "zone"   "gmtoff"

$class
[1] "POSIXlt" "POSIXt" 

$tzone
[1] ""    "CST" "CDT"
#mode函数
> mode(m_time_2)
[1] "list"
#class函数
$class
[1] "POSIXlt" "POSIXt" 
#names函数
> names(unclass(m_time_2))
 [1] "sec"    "min"    "hour"   "mday"   "mon"    "year"   "wday"   "yday"   "isdst" 
[10] "zone"   "gmtoff"
```
## 属性的获取和设置 ##
函数attributes(object)将给出当前对象所具有的所有非基本属性（长度和模式属于基本属性）的一个列表。

##  对象的类别 ##
对象的一个特别属性，类别，被用来指定对象在R编程中的风格。比如：如果对象类别"data.frame"则会以特定方式处理。 (其实就是下面的类型用 class()函数获取)用unclass()函数可以去除对象的类别。
## 数据类型 ##
基本的数据类型

|类型|示例|
|------|:-----:|
|逻辑|TRUE、FALSE|
|数字|1,2,3|
|整数|2L,3L,4L|
|复数|3+2i|
|字符|'a','good'|

由基本对象组成的 R-对象
**向量**：使用`c()`函数创建(只能包含同一类型的对象)
```R
# Create a vector.
apple <- c('red','green',"yellow")
print(apple)

# Get the class of the vector.
print(class(apple))

#输出
[1] "red"    "green"  "yellow"
[1] "character" 
```
**列表**:它里面可以包含多个不同类型的元素，如向量，函数，甚至是另一个列表。`list()`函数生成
```R
#Create a list
m_list<-list(c(2,5),21.3,TRUE)
print(m_list)
#输出
[[1]]
[1] 2 5

[[2]]
[1] 21.3

[[3]]
[1] TRUE
```
**矩阵**：二维矩形数据集。它可以使用一个向量输入到矩阵函数来创建`matrix()`函数生成
```R
# Create a matrix
M<-matrix(c('a','a','b','c','b','a'),nrow = 2,ncol = 3,byrow = TRUE)
print(M)
#输出
     [,1] [,2] [,3]
[1,] "a"  "a"  "b" 
[2,] "c"  "b"  "a" 
```
**数组**:数组可以是任何数目的尺寸大小。数组函数使用它创建维度的所需数量的属性-dim 用`array()`来生成
```R
#Create a array
m_array<-array(c('a','b'),dim = c(3,3,2))
print(m_array)

#输出
, , 1

     [,1] [,2] [,3]
[1,] "a"  "b"  "a" 
[2,] "b"  "a"  "b" 
[3,] "a"  "b"  "a" 

, , 2

     [,1] [,2] [,3]
[1,] "b"  "a"  "b" 
[2,] "a"  "b"  "a" 
[3,] "b"  "a"  "b" 
```

**因子**：R语言中的因子就是factor，用来表示分类变量(categorical variables)，这类变量不能用来计算而只能用来分类或者计数。也就是这个向量中有多少类别
可以排序的因子称为有序因子(ordered factor)。
用来生成因子数据对象，语法是：
factor(data, levels, labels, ...)
其中data是数据，levels是因子的级别向量，labels是因子的标签向量。
```R
m_factor<-c('a','v','a','b','b','A','V','v')
factor_appel<-factor(m_factor)
print(factor_appel)
print(nlevels(factor_appel))

#输出
[1] a v a b b A V v
Levels: a A b v V
> print(nlevels(factor_appel))
[1] 5
```

因子到向量的转换
```R
#新建一个因子
q<-factor(c("boy","gril","boy","gril","boy","gril"),levels = c("boy","gril"))
q
#以表格的形式显示
table(q)
#因子转换为向量
q2<-as.vector(q)
q2
#因子转化为数字
q3<-as.numeric(q)
q3

#输出信息
[1] boy  gril boy  gril boy  gril
Levels: boy gril
#表格
> table(q)
q
 boy gril 
   3    3 
#向量
> q2<-as.vector(q)
> q2
[1] "boy"  "gril" "boy"  "gril" "boy"  "gril"
#数字
> q3<-as.numeric(q)
> q3
[1] 1 2 1 2 1 2
```
**数据帧**：每一列的数据类型相同。第一列可以是数字，而第二列可能是字符和第三列可以是逻辑。它与向量列表的长度相等。用函数`data.frame()`
```R
BMI <- 	data.frame(
  gender = c("Male", "Male","Female"), 
  height = c(152, 171.5, 165), 
  weight = c(81,93, 78),
  Age =c(42,38,26)
)
print(BMI)
#输出
  gender height weight Age
1   Male  152.0     81  42
2   Male  171.5     93  38
3 Female  165.0     78  26
```

**日期与时间**距离1970-01-01的天数
相应的函数`date()`是字符串类型的
Date类型`Sys.Date()`
存储日期as.Date("2016-01-01")


```R
x<-date()
print(x)
x2<-Sys.Date();
x2
x3<-as.Date("2016-01-01")
x3
#输出
[1] "Fri Oct 07 20:21:21 2016"
> x2<-Sys.Date();
> x2
[1] "2016-10-07"
> x3<-as.Date("2016-01-01")
> x3
[1] "2016-01-01"
#字符串转化为时间类型
> as.Date('April 26, 2001',format='%B %d, %Y')
[1] "2001-04-26"
```
**时间**:距离1970-01-01的秒数/Sys.time()
- POSIXct:以有符号整数形式存储，表示从1970-01-01到该时间点经过的秒
- POSIXlt:以字符串 列表 形式存储，包含年月日等
```R
#新建一个时间变量，输出类型，再将格式转化为lt类型
m_time<-Sys.time()
print(m_time)
class(m_time)
m_time_2<-as.POSIXlt(m_time)
class(m_time_2)
# 查看所有的名字属性可以用m_time2.sex等进行对应的值的获取
names(unclass(m_time_2))
#输出信息
> m_time<-Sys.time()
> print(m_time)
[1] "2016-10-09 21:08:28 CST"
> class(m_time)
[1] "POSIXct" "POSIXt" 
> m_time_2<-as.POSIXlt(m_time)
> class(m_time_2)
[1] "POSIXlt" "POSIXt" 
> names(unclass(m_time_2))
 [1] "sec"    "min"    "hour"   "mday"   "mon"    "year"   "wday"   "yday"   "isdst" 
[10] "zone"   "gmtoff"

#对应值的获取
m_time_2$hour
#输出信息> m_time_2$hour
[1] 21
```

## 将不标准的时间字符串转变为日期类型 ##
(还不是很明白)？
```R
x <- "一月 1,2015 02:01"
x11 <- strptime(x,"%B %d,%Y %H:%M")
x11
#输出
> x11 <- strptime(x,"%B %d,%Y %H:%M")
> x11
[1] "2015-01-01 02:01:00 CST
```
|格式|意义|
|------|:-----:|
|%Y|年份，以四位数字表示，2007|
|%y|年份，以两位位数字表示，07|
|%m|月份，以数字形式表示，从01到12|
|%B|月份，完整的月份名，指英文February(也指中文的一月)|
|%b|月份，缩写，Feb|
|%d|月份中当的天数，从01到31|
|%H|小时|
|%M|秒|

## 小结 ##
![](http://i.imgur.com/UstGJpX.png)

# 构建子集 #
----------
## 矩阵的子集 ##
取一行或者一列
vector()+dim()也可以创建一个矩阵
cbind(),rbind()两个矩阵拼  接连起来
````R
#新建矩阵
 x<-matrix(c("1","2"),nrow=2,ncol=3, )
#输出第二行，第二个元素
 x[2,2]
输出第二行整行
 x[2,,drop=FALSE]
#输出信息
> x[2,2]
[1] "2" 

> x[2,]
[1] "2" "2" "2"
#输出一整行 以矩阵的形式表示
> x[2,,drop=FALSE]
     [,1] [,2] [,3]
[1,] "2"  "2"  "2" 

#输出加条件
#输出第二行的第一个和第三个
 x[2,c(1,3)]
#输出信息
> x[2,c(1,3)]
[1] "2" "2"
```

## 数据框的子集 ##
其实和矩阵差不多:
```R
#新建一个数据框
x<-data.frame(v1=1:5,v2=c("a","b","c","d","e"),v3=c(TRUE,TRUE,TRUE,FALSE,FALSE))
#输出数据框的第一列
x$v1
x[,1]
#
x[(x$v1>2 & x$v1<4),]

#输出信息
> x$v1
[1] 1 2 3 4 5

> x[,1]
[1] 1 2 3 4 5

> x[(x$v1>2 & x$v1<4),]
  v1 v2   v3
3  3  c TRUE

#which函数。。。
x[which(x$v1>2 & x$v1<4),]
#输出  和不加which一样
> x[which(x$v1>2 & x$v1<4),]
  v1 v2   v3
3  3  c TRUE
```
which()函数返回的是公式 TRUE对应的向量下标
subset()函数取子集
```R
subset(x,x$v1>2)
#输出
> subset(x,x$v1>2)
  v1 v2    v3
3  3  c  TRUE
4  4  d FALSE
5  5  e FALSE
```

## 列表的子集 ##
取列表中的元素
```R
x<-list(id=1:4,hegiht=178,sex="male")
x[1]
x["id"]

#输出
> x<-list(id=1:4,hegiht=178,sex="male")
> x[1]
$id
[1] 1 2 3 4

> x["id"]
$id
[1] 1 2 3 4
```
去除列表中元素的内容
```R
x[["id"]]
x[[1]]
x$id
#输出信息 三个效果相同 
> x[["id"]]
[1] 1 2 3 4

> x[[1]]
[1] 1 2 3 4

> x$id
[1] 1 2 3 4

> x$id
[1] 1 2 3 4
```
嵌套的子集
```R
#新建list，在list中元素还是list
x<-list(a=list(1,2,3,4),b=c("haha","xixi"))
#用嵌套
x[[1]][[1]]
#用c()函数
x[[c(1,1)]]
#输出

> x[[1]][[1]]
[1] 1

> x[[c(1,1)]]
[1] 1
```
不完全匹配
```R
l<-list(ashkf=1:10)
l$a
l[["a"]]
l[["a",exact=FALSE]]
#输出
> l$a
 [1]  1  2  3  4  5  6  7  8  9 10

> l[["a"]]
NULL

> l[["a",exact=FALSE]]
 [1]  1  2  3  4  5  6  7  8  9 10
```
## 处理缺失值 ##
```R
#NA表示确实值
y<-c(1,NA,2,3,NA)
#缺失值FALSE,其他TRUE
is.na(y)
#去除缺失值
y[!is.na(y)]

#输出
> is.na(y)
[1] FALSE  TRUE FALSE FALSE  TRUE

> y[!is.na(y)]
[1] 1 2 3
```

complete.cases()函数x,y对应位置上都不是缺失值
```R
x<-c(1,2,NA,NA)
y<-c(NA,"a",NA,1)
z<-complete.cases(x,y)
z
#输出
[1] FALSE  TRUE FALSE FALSE
```
## 向量化操作 ##


----------
# 重要的函数 #
**lapply**(参数):lapply(列表，函数/函数名，其他参数)(其实就是对list中每个元素进行相同的操作再输出为一个list)
返回参数还是个列表
如果 给定参数不是列表，lapply函数会将它强制转化为列表

lapply(X, FUN, ...)
lapply的返回值是和一个和X有相同的长度的list对象，这个list对象中的每个元素是将函数FUN应用到X的每一个元素。其中X为List对象（该list的每个元素都是一个向量），其他类型的对象会被R通过函数as.list()自动转换为list类型。

str()函数 ：简洁的函数定义
```R
x<-list(a=matrix(1:6,2,3),b=matrix(4:7,2,2))
x
lapply(x, function(m) m[1,])
#输出
> x<-list(a=matrix(1:6,2,3),b=matrix(4:7,2,2))
> x
$a
     [,1] [,2] [,3]
[1,]    1    3    5
[2,]    2    4    6

$b
     [,1] [,2]
[1,]    4    6
[2,]    5    7

#用了一个匿名函数
> lapply(x, function(m) m[1,])
$a
[1] 1 3 5

$b
[1] 4 6

```
sapply:简化结果
- 结果列表元素均为1，返回向量。
- 结果列表元素类型相同且大于1，返回矩阵。


**apply函数**对一个数组按行或者按列进行计算）：沿着数组的某一维度处理数据 维度若为1表示取行，为2表示取列，为c(1,2)表示行、列都计算。数组
例如将函数用于矩阵的行或列
apply(参数)：apply(数组，维度，函数/函数名)
```R
x<-matrix(1:4,3,4)
x
apply(x, 1, mean)

#输出
> x<-matrix(1:4,3,4)
> x
     [,1] [,2] [,3] [,4]
[1,]    1    4    3    2
[2,]    2    1    4    3
[3,]    3    2    1    4
> apply(x, 1, mean)
[1] 2.5 2.5 2.5
```
mapply:mapply(函数/函数名，数据，函数相关的参数)
```R
list(rep(1,4),rep(2,3),rep(3,2))
mapply(rep,1:4,4:1)
#输出
> list(rep(1,4),rep(2,3),rep(3,2))
[[1]]
[1] 1 1 1 1

[[2]]
[1] 2 2 2

[[3]]
[1] 3 3

> mapply(rep,1:4,4:1)
[[1]]
[1] 1 1 1 1

[[2]]
[1] 2 2 2

[[3]]
[1] 3 3

[[4]]
[1] 4
```

**tapply:对向量的子集进行操作（进行分组统计）**
tapply(向量，因子/因子列表,函数/函数名)
gl()函数：产生因子变量gl(n,k,labels=c("高"，"中","低"))
n:表示级别数
k:每个级别重复次数
labels:可选的向量因子水平标签

tapply(X, INDEX, FUN = NULL, ..., simplify = TRUE)
其中X通常是一向量；INDEX是一个list对象，**且该list中的每一个元素都是与X有同样长度的因子**(此处是取对应位置的下标,如果x是一个数据框的话就比较好理解了)；FUN是需要计算的函数；simplify是逻辑变量，若取值为TRUE（默认值），且函数FUN的计算结果总是为一个标量值，那么函数tapply返回一个数组；若取值为FALSE，则函数tapply的返回值为一个list对象。需要注意的是，当第二个参数INDEX不是因子时，函数 tapply() 同样有效，因为必要时 R 会用 as.factor()把参数强制转换成因子。
```R
a<-data.frame(name=c("yth1","yth2","yth3","yth4"),age=c(8,8,9,9),math=c(100,50,70,40))
tapply(a[,"math"], a[,"age"], mean)

#输出
 8  9 
75 55 
```
如果是向量的话

```R
#因子与student等长
class<-c(1,2,3,2,1,2,1,3)
c(81,65,72,88,73,91,56,90)->student
factor(class)->class
table(class)
tapply(student,class,mean)
#输出
> table(class)
class
1 2 3 
3 3 2 

> tapply(student,class,mean)
       1        2        3 
70.00000 81.33333 81.00000 
```
**函数table（求因子出现的频数）**
split()函数：
	- 根据因子或因子列表，将向量或其他对象分组
	- 通常与lapply一起使用
	- split(参数)：split(向量/列表/数据框,因子/因子列表)，返回值是list

简单实例
```R
#新建因子
ma<-matrix(c(1:4, 1, 6:8), nrow = 2)
ma<-c(1,2,1,3,2,2,1)
ma<-factor(ma)
#按因子分割
split(c(1:7),ma)

#输出
> split(c(1:7),ma)
$`1`
[1] 1 3 7

$`2`
[1] 2 5 6

$`3`
[1] 4
```

结合lapply()使用
```R
head(airquality)
lx<-split(airquality,airquality$Month)
lapply(lx, function(x) mean(x[,"Temp"]))

#输出 每个月的平均气温
$`5`
[1] 65.54839

$`6`
[1] 79.1

$`7`
[1] 83.90323

$`8`
[1] 83.96774

$`9`
[1] 76.9
```

**rnorm**(n, mean = 0, sd = 1)
n 为产生随机值个数（长度），mean 是平均数， sd 是标准差 。