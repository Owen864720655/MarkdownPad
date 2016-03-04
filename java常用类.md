## String ##
String 类与StringBuffer的区别
- String 类是不可变的类，String对象创建后它的内容无法改变,一些看起来改变的方法实际上是新创建了一个String对象。
StringBuffer会改变字符缓冲区的字符串内容.
- String 类覆盖了equals()方法，StringBuffer没有覆盖
- StringBuffer不能用+号

-------------
**包装类**   把基本类型转化为对象
## Random类 ##
- nextInt()返回一个int类型的随机数在0.0-1.0之间
类似的还有Long,Float,Double,Boolean等类型

## Date类 ##
```java
Date date = new Date();
//DateFormat抽象类格式化日期输出
DateFormat df=new SimpleDateFormat("yy年MM月dd日 hh:mm:ss");
		System.out.println(df.format(date));
```
```java
//按照特殊的格式将String转化为Date对象,df格式必须与String参数的格式一致
Date d1=df.parse("")
```
## Calendar ##
```java
Calendar ca=Calendar.getInstance();
ca.各个字段获取时间的字段,然后通过ca.get()获取具体的时间
ca.get(ca.YEAR)

```
## Class类 ###
  获取Class实例的三种方式：
- 利用对象调用getClass()方法获取该对象的Class实例；
- 使用Class类的静态方法forName()，用类的名字获取一个Class实例
- 运用.class的方式来获取Class实例，对于基本数据类型的封装类，还可以采用.TYPE来获取相对应的基本数据类型的Class实例
```java
String str1="abc";
 Class cls1=str1.getClass();
 Class cls2=String.class;
 Class cls3=Class.forName("java.lang.String");
```