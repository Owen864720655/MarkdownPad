#Spring MVC请求参数
##使用@RequestMapping映射请求##
Spring MVC 使用 @RequestMapping 注解为控制器指定可
以处理哪些 URL 请求(相当于是配置请求拦截，相应的请求在相应的Controller里边处理)
###@RequestMapping
- 类定义处：路径相当于web应用的根目录一级映射
- 方发处，如果类出有定义，则此处吃二级映射，否则一级映射，相对于web根目录

@RequestMapping 的 value、method、params 及 heads 
分别表示请求 URL、请求方法、请求参数及请求头的映射条
件，他们之间是与的关系，联合使用多个条件可让请求映射
更加精确化
```java
@RequestMapping(value = "/hello")
	public ModelAndView showMessage(@RequestParam(value = "name", required = false, defaultValue = "World") String name,
			@RequestParam(value="age",required=false,defaultValue="22") int age) {
		ModelAndView mv = new ModelAndView("helloworld");
		mv.addObject("message", message);
		mv.addObject("name", name);
		mv.addObject("age", 22);
		return mv;
	}
```

##绑定数据的注解##
1. @RequestParam，绑定单个请求数据，可以是URL中的数据，表单提交的数据或上传的文件； 
2. @PathVariable，绑定URL模板变量值； 
3. @CookieValue，绑定Cookie数据； 
4. @RequestHeader，绑定请求头数据； 
5. @ModelAttribute，绑定数据到Model； 
6. @SessionAttributes，绑定数据到Session； 
7. @RequestBody，用来处理Content-Type不是application/x-www-form-urlencoded编码的内容，例如application/json, application/xml等； 
8. @RequestPart，绑定“multipart/data”数据，并可以根据数据类型进项对象转换；


###@PathVariable 映射 URL 绑定的占位符
```java
	@RequestMapping("/path/{id}")
	public String path(@PathVariable("id")int id){
		System.out.println(id);
		return "success";
	}
```
###@RequestParam###
在处理方法入参处使用@RequestParam 可以把请求参数传递给请求方法
- value：参数名
- required：是否必须。默认为 true, 表示请求参数中必须包含对应
的参数，若不存在，将抛出异常
- defaultValue 默认值
###@RequestHeader###
使用 @RequestHeader 绑定请求报头的属性值,通过 @RequestHeader 即可将请求头中的属性值绑
定到处理方法的入参中
##使用POJO对象绑定请求参数##
 Spring MVC 会按请求参数名和 POJO 属性名进行自动匹配，自动为该对象填充属性值。支持级联属性
  注：**表单中input的name值和Controller的参数变量名保持一致，就能完成数据绑定**，如果不一致可以使用@RequestParam注解。需要注意的是，如果Controller方法参数中定义的是基本数据类型，但是从页面提交过来的数据为null或者”"的话，会出现数据转换的异常。也就是必须保证表单传递过来的数据不能为null或”"，所以，在开发过程中，对可能为空的数据，最好将参数数据类型定义成包装类型.
###基本的对象###
```java
public class User {
    private String firstName;
    private String lastName;
    public String getFirstName() {
        return firstName;
    }
    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }
    public String getLastName() {
        return lastName;
    }
    public void setLastName(String lastName) {
        this.lastName = lastName;
    }
}
```
Controller代码
```java
@RequestMapping("info")
public void test(User user) {
}
```
表单代码
```html
<form action="info" method="post">
<input name="firstName" value="张" type="text"/>
<input name="lastName" value="三" type="text"/>
</form>
```
###List/Set/Map###
List需要绑定在对象上，而不能直接写在Controller方法的参数中。如下：
```java
public class UserListForm {
    private List<User> users;
    public List<User> getUsers() {
        return users;
    }
    public void setUsers(List<User> users) {
        this.users = users;
    }
}
```
html中：
```html
<tr>
<td><input name="users[0].firstName" value="aaa" /></td>
<td><input name="users[0].lastName" value="bbb" /></td>
</tr>
<tr>
<td><input name="users[1].firstName" value="ccc" /></td>
<td><input name="users[1].lastName" value="ddd" /></td>
</tr>
<tr>
<td><input name="users[2].firstName" value="eee" /></td>
<td><input name="users[2].lastName" value="fff" /></td>
</tr>
```
注意:其实，这和第4点User对象中的contantInfo数据的绑定有点类似，但是这里的UserListForm对象里面的属性被定义成List，而不是普通自定义对象。所以，在表单中需要指定List的下标。值得一提的是，Spring会创建一个以最大下标值为size的List对象，所以，如果表单中有动态添加行、删除行的情况，就需要特别注意，譬如一个表格，用户在使用过程中经过多次删除行、增加行的操作之后，下标值就会与实际大小不一致，这时候，List中的对象，只有在表单中对应有下标的那些才会有值，否则会为null
其他类似。
##输出模型数据##
- ModelAndView: 处理方法返回值类型为 ModelAndView 时, 方法体即可通过该对象添加模型数据
- **Map 及 Model: 入参为 
org.springframework.ui.Model、org.springframework.ui.ModelMap 或 java.uti.Map 时，处理方法返回时，Map 中的数据会自动添加到模型中。**
- @SessionAttributes: 将模型中的某个属性暂存到 HttpSession 中，以便多个请求之间可以共享这个属性
- @ModelAttribute: 方法入参标注该注解后, 入参的对象就会放到数据模型中
###ModelAndView###
控制器处理方法的返回值如果为 ModelAndView, 则其既包含视图信息，也包含模型数据信息。
 添加模型数据:
- MoelAndView addObject(String attributeName, Object attributeValue)
- ModelAndView addAllObject(Map<String, ?> modelMap)
设置视图:
- void setView(View view)
- void setViewName(String viewName)




###SessionAttributes###
若希望在多个请求之间共用某个模型属性数据，则可以**在控制器类上**标注一@SessionAttributes, Spring MVC 将在模型中对应的属性暂存到 HttpSession 中。

•@SessionAttributes 除了可以通过属性名指定需要放到会话中的属性外，还可以通过模型属性的对象类型指定哪些模型属性需要放到会话中
- @SessionAttributes(types=User.class)
为 User.class 的属性添加到会话中。
- @SessionAttributes(value={“user1”,
- @SessionAttributes(types={User.class,
- @SessionAttributes(value={“user1”,types={Dept.class})

```html
${requestScope.user } 
${sessionScope.user }
```
###ModelAttribute###
 在**方法定义上**使用 @ModelAttribute 注解：Spring MVC 在调用目标处理方法前，会先逐个调用在方法级上标注了 @ModelAttribute 的方法。

 在**方法的入参**前使用 @ModelAttribute 注解：
- 可以从隐含对象中获取隐含的模型数据中获取对象，再将请求参数绑定到对象中，再传入入参
- 将方法入参对象添加到模型中(即通过requestScope.user来获取Model值(user即为@ModelAttribute的value值))
![](http://i.imgur.com/FCwZHTF.png)
在部分更新的时候可以用，先通过注解了@ModelAttribute的方法中获取要更新的对象。然后更新的时候部对象赋值，部分更新。

Spring MVC确定目标方法POJO类型入参的过程。
![](http://i.imgur.com/Jois1ZD.png)
源码分析
![](http://i.imgur.com/BkckJ4A.png)

#Spring MVC解析视图