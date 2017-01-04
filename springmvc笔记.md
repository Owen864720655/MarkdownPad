# SpringMVC笔记 #
----------
## 关于RequestMapping ##


> 可以作用域类名和方法名,作用于方法，则相对于类目录，如果类目录没有 @RequestMapping的话，就是相对于web根目录。

@RequestMapping 的 value、method、params 及 heads 分别表示请求 URL、请求方法、请求参数及请求头的映射条件。
value的值加不加'/'都可以
```java
	@RequestMapping(value = "methodPost", method = RequestMethod.POST,params={"name","age"})
	public String testMethod() {
		return "success";
	}
```

`params`和`headers`一些简单表达式
- param1: 表示请求必须包含名为 param1 的请求参数
- !param1: 表示请求不能包含名为 param1 的请求参数
- param1 != value1: 表示请求包含名为 param1 的请求参数，但其值不能为 value1
- {“param1=value1”, “param2”}: 请求必须包含名为 param1
的两个请求参数，且 param1 参数的值必须为 value1

### @PathVariable映射 URL 绑定的占位符　 ###

```java
//@PathVariable 可以用来映射URL中的占位符到目标方法的参数中。
@RequestMapping("/haha/{id}/{name}")
	public String pathtest(@PathVariable("id")Integer id,@PathVariable("name")String name) {
		System.out.println(id+name);
		return "success";
	}
```

### REST(Representational State Transfer)风格 ###
**解释：**
1. 每一个URI代表一种资源；
2. 客户端和服务器之间，传递这种资源的某种表现层；(txt,json...)
3. 客户端通过四个HTTP动词，对服务器端资源进行操作，实现"表现层状态转化"。

**示例：**
- /order/1 HTTP GET ：得到 id = 1 的 order
- /order/1 HTTP DELETE：删除 id = 1的 order
- /order/1 HTTP PUT：更新id = 1的 order
- /order HTTP POST：新增 order 


注意:路径相关 http://www.cnblogs.com/w-wfy/p/5598098.html
重定向后路径。

配置流程

1. 最好放在web.xml开头
```xml
<!-- 配置 org.springframework.web.filter.HiddenHttpMethodFilter：可以把POST请求转为DELETE或PUT -->
	<filter>
		<filter-name>HiddenHttpMethodFilter</filter-name>
		<filter-class>org.springframework.web.filter.HiddenHttpMethodFilter</filter-class>
	</filter>

	<filter-mapping>
		<filter-name>HiddenHttpMethodFilter</filter-name>
		<url-pattern>/*</url-pattern>
	</filter-mapping>
```
2. java中
在tomcat8中会报405错误`JSPs only permit GET POST or HEAD`(不算个错误)
```java
	@RequestMapping(value = "/delete/{id}", method = RequestMethod.DELETE)
		public String delete(@PathVariable("id") Integer id) {
			System.out.println("请求方法" + "DELETE" + id);
			return "redirect:/index";
	
		}

	@RequestMapping(value = "/put/{id}", method = RequestMethod.PUT)
	public String put(@PathVariable("id") Integer id) {
		System.out.println("请求方法" + "PUT" + id);
		return "success";

	}
```

3.调用
需要一个hidden表单。name="_method",value="PUT"
```jsp
	<form action="all/put/1" method="post">
		<input type="hidden" name="_method" value="PUT" /> <input
			type="submit" value="put" />
	</form>
	<form action="all/delete/1" method="post">
		<input type="hidden" name="_method" value="DELETE" /> <input
			type="submit" value="delete" />
	</form>
```
> 方法的请求参数，既可以在@RequestMapping中 指定，也可以在方法的参数中指定。

### 请求参数 @RequestParam、@RequestHeader 等###
```java
	@RequestMapping("/testP2")
	public String params2(@RequestParam("name")String name,@RequestParam(value="age",required=false)Integer age) {
		System.out.println("姓名："+name+"年龄"+age);
		return "success";
	}
```

```java
	@RequestMapping("/head")
	public String header( @RequestHeader ("Accept") String acceptType) {
		System.out.println("Accept : " + acceptType);  
		return "success";
	}
```
## 使用POJO对象绑定请求参数值 ##

## Map 及 Model ##
```java
	@RequestMapping("/testMap")
	public String testMap(Map<String,Object>map) {
		map.put("name", "ythww");
		return "success2";
	}
```

```jsp
<h2>name:${requestScope.name}</h2>
```

## @SessionAttributes ##
若希望在多个请求之间共用某个模型属性数据，则可以在**控制器类**上标注一个 @SessionAttributes, Spring MVC 将在模型中对应的属性暂存到 HttpSession 中。
- @SessionAttributes(types=User.class)为 User.class 的属性添加到会话中。
- @SessionAttributes(value={“user1”,
- @SessionAttributes(types={User.class,
- @SessionAttributes(value={“user1”,types={Dept.class})


## @ModelAttribute ## 
(以后再看，大概知道什么意思)
http://blog.csdn.net/z69183787/article/details/48951721

执行流程:
1. 执行@ModelAttribute 注解修饰的方法:从数据库中取出对象，把对象放入Map中，键为:user。
2. SpringMVC从Map中取出User对象，并把表单的请求参数赋给User对象的对应属性。
3. SpringMVC把上述对象传入目标方法的参数。

具体流程：
1. @ModelAttribute中map键值对 放入implicitModel中
2. 解析请求处理器的目标参数
	1. 创建	WebDataBinder对象(两个参数):
		1. objectName:如果目标参数中没有用@ModelAttribute修饰的话，objectName为类名第一个字母小写，如果有则为@ModelAttribute的value属性。
		2. 确定target：在implicitModel中查找objectName,若有，返回，若不存在则验证是否用@SessionAttributes修饰objectName，若有，从sesion中取出，若sesion中不存在，则抛出异常(注意:抛出异常条件，首先Controller被@SessionAttributes修饰，其次，它的value中没有objectName。)若都没有，则通过反射创建。
3. 将表单赋值给target
4. 把target作为目标参数入参。