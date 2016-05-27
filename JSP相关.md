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
对javaBean操作
```java
	姓名${requestScope.user.name }//用点操作
	性别${requestScope.user["sex"] }//用[ ]操作,注意括号里边的属性要双引号
	邮箱${requestScope.user.email }
```
### 对Map操作类似 ###
Servlet中
```java
	HashMap<String, Object>map=new HashMap<>();
	map.put("name", "姚腾辉");
	map.put("love", "刘妍");
	request.setAttribute("map", map);
	request.getRequestDispatcher("/views/show.jsp").forward(request, response);
```

```java
   ${requestScope.map.name }Love${requestScope.map["love"]}
```
###  List操作  ###
```java
	java.util.List<String> list = new ArrayList<>();
	list.add("姚腾辉01");
	list.add("姚腾辉02");
	list.add("姚腾辉03");
	request.setAttribute("list", list);
	request.getRequestDispatcher("/views/show.jsp").forward(request,
	response);
```
```jsp
	${requestScope.list[0]}
	${requestScope.list[1]}
	${requestScope.list[2]}
```
List还有数组都可以通过索引来获取。索引里面数字不用加引号

## 对象域 ##
如果没有指定作用域
pageScope->requestScope->sessionScope->applicationScope这样的顺序查找。
# JSTL #
需要导入jar包
需要taglib进行声明
```html
<%@taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
```
`c:forEach`控制循环标签
```jsp
	<c:forEach begin="1" end="10" step="1">
	Hello!
	</c:forEach>

	<c:forEach var="i" begin="1" end="10" step="1">
	${i}
	</c:forEach>
```

对List操作

```java
		User user1 = new User();
		user1.setName("姚腾辉01");
		user1.setSex("男");
		user1.setEmail("110132@qq.com");
		
		User user2 = new User();
		user2.setName("姚腾辉02");
		user2.setSex("男");
		user2.setEmail("110132@qq.com");

		list.add(user1);
		list.add(user2);
		request.setAttribute("list", list);
		request.getRequestDispatcher("/views/show.jsp").forward(request, response);
```


```jsp
	<c:forEach var="user" items="${requestScope.list }">
	${user.name}
	${user.sex }
	${user.email }
```

`<c:forEach></c:forEach>`标签属性及作用

- var 代表循环对象的变量名，若存在items属性，则表示循环对象的变量名。
- items 进行循环的集合
- varStatus显示循环的状态变量
	- current当前这次迭代的（集合中的）项
	- index当前这次迭代从 0 开始的迭代索引
	- count当前这次迭代从 1 开始的迭代计数
	- first用来表明当前这轮迭代是否为第一次迭代的标志
	- last用来表明当前这轮迭代是否为最后一次迭代的标志
	- begin属性值
	- end属性值
	- step属性值 
- begin 开始条件
- end结束条件
- step 循环的步长