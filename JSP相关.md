## JavaBean ##
在Servlet中
```java
request.setAttribute("user", user);
```
在jsp中
```jsp
	<jsp:useBean id="user" class="com.yth.jsp.User"/>
	<jsp:getProperty property="name" name="user"/>
	<jsp:getProperty property="password" name="user"/>
```
其实jsp的操作可以被EL替换掉。在集合的处理上javaBean无能为力。
## EL表达式的使用 ##
EL设计的主要目的是简化页面输出`${ }`代码写在花括号中就行。
可以输出常量，变量。也可以直接写在表达式内部的。
```jsp
<input type="text" placeholder="${'请输入'}">
<p>${"姚腾辉" }</p>
```
EL中与作用域相关的隐含对象

隐含变量|类型|作用
----|----|----
pageScope|java.util.map|可在页面范围内查找某个属性名对应的值
requestScope|java.util.map|可在请求范围内查找某个属性名对应的值
sessionScope|java.util.map|可在会话范围内查找某个属性名对应的值
applicationScope|java.util.map|可在应用范围内查找某个属性名对应的值

## .操作与[ ]操作。##
