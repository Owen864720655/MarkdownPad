#Spring注解记录

----------
##声明切面
@Aspect注解的Java类为一个代理类
Bean 配置文件中定义一个空的 XML 元素 <aop:aspectj-autoproxy>
@Order指定切面打的优先级 参数越小优先级越大

##通知(切面里面的一个方法)


- @Before: 前置通知, 在方法执行之前执行
- @After: 后置通知, 在方法执行之后执行 
- @AfterRunning: 返回通知, 在方法返回结果之后执行
- @AfterThrowing: 异常通知, 在方法抛出异常之后
- @Around: 环绕通知, 围绕着方法执行

##重用切入点定义
@pointcut:重用切入点，下面的只有空方法

##使用外部属性文件
在beans中配置``` <context:property-placeholder>```配```location属性```
在 Bean 配置文件里使用形式为 ${var} 的变量

##Aop记录
###使用基于AspectJ 的注解
要在 Spring IOC 容器中启用 AspectJ 注解支持, 只要在 Bean 配置文件中定义一个空的 XML 元素 <aop:aspectj-autoproxy>
###AspectJ 支持 5 种类型的通知注解: 


- @Before: 前置通知, 在方法执行之前执行
- @After: 后置通知, 在方法执行之后执行 
- @AfterRunning: 返回通知, 在方法返回结果之后执行
- @AfterThrowing: 异常通知, 在方法抛出异常之后
- @Around: 环绕通知, 围绕着方法执行


```java
		try{
			//前置通知
			informRule();
			//返回通知
		}catch RuntimeException{
			//异常通知
		}
		//后置通知

```
##通过注解的方式配置切面 及细节
具体每个通知的用法参考ppt 87页之后
1. 要以一个类被声明为```@Aspect```
2. Bean 配置文件中定义一个空的 XML 元素 ```<aop:aspectj-autoproxy>```
3. 编写具体的通知方法,添加注解```@Before("execution(* *.*(..))")```其中 ```execution()```里边是具体作用的方法范围.
- 可以在通知方法中声明一个类型为 JoinPoint 的参数,通过JoinPoint参数来获取连接点的细节
```joinPoint.getSignature().getName()```获取方法名
```joinPoint.getArgs()```获取方法参数
- 返回通知可以获取到 方法的返回值
```	java
@AfterReturning(value="execution(* com.spring2.aop.imp.ArithmeticCalculatorIn(int,int))",returning="result")
	//@AfterReturning("informRule()")
	public void afterResult(JoinPoint joinPoint,Object result) {
		System.err.println("正常返回通知"+result);
	}
```
在注解中声明中 添加```returnning```属性，在方法的参数中添加```Object```参数，该参数就是返回值

- 对于异常通知，也可以获取抛出的异常信息
在注解声明中添加```throwing```属性
```java
	@AfterThrowing(pointcut = "execution(* com.spring2.aop.imp.ArithmeticCalculatorIn.*(int,int))", throwing = "e")
	public void errorResult(JoinPoint joinPoint, Throwable e) {
		System.out.println("发生异常" +  joinPoint.getSignature().getName()+e);
	}
```
- 当有多个切面时可以通过在切面类中什么```Order(x)``来指定切面优先级，其中值越小优先级越大


##通过XML配置切面
1. 首先建一个封装通知的类`LogCatXml`
2. 在xml文件中配置
- 先配置通知封装类

 ```
 	<!-- 配置LogCatXml的Bean -->
	<bean id="logCatXml" class="com.spring2.aop.imp.LogCatXml"></bean>
  ```
``` xml
	<!-- 声明切面 -->
	<aop:config>
		<aop:aspect id="logAspect" ref="logCatXml">
			<!-- 配置切点，在 <aop:config>下配置对所有的切面生效 -->
			<aop:pointcut
				expression="execution(* com.spring2.aop.imp.ArithmeticCalculatorIn.*(int,int))"
				id="testOperation" />
			<!-- 配置通知 -->
			<!-- 前置通知 -->
			<aop:before method="beforeMethod" pointcut-ref="testOperation" />
			<!-- 返回通知 -->
			<aop:after-returning method="afterResult"
				pointcut-ref="testOperation" returning="result" />
			<!-- 后置通知 -->
			<aop:after method="after" pointcut-ref="testOperation" />
			<!-- 异常通知 -->
			<aop:after-throwing method="errorResult"
				pointcut-ref="testOperation" throwing="e" />
		</aop:aspect>
	</aop:config>
```
##Spring中的事物管理##
###基本配置
1. xml中的配置

```xml
<!-- 配置事物管理器 datasource为数据库的源的Bean-->
	<bean id="transactionManager"
		class="org.springframework.jdbc.datasource.DataSourceTransactionManager">
		<property name="dataSource" ref="datasource"></property>
	</bean>

	<!-- 启用事物注解 -->
	<tx:annotation-driven transaction-manager="transactionManager" />
```
2.在要进行事物管理的方法前面加上`@Transactional`

###事物的传播行为REQUIRED
当一个被`@transational`注解的方法，调用另外一个被`@Transactional`注解的方法时就会出现事物的传播行为
可以在`@Transactional(propagation = Propagation.REQUIRED)`中设置`propagation`的属性来配置是否启用新事物

![事物传播的属性设置](http://i.imgur.com/sCeiXEd.png)


##基于xml配置的事务
还不是很懂
```xml
<!-- 配置事物管理器 -->
	<bean id="transactionManager"
		class="org.springframework.jdbc.datasource.DataSourceTransactionManager">
		<property name="dataSource" ref="datasource"></property>
	</bean>

<!-- 配置事物属性是一个通知 --> 
	<tx:advice id="bookTx" transaction-manager="transactionManager">
		<tx:attributes>
			<tx:method name="*" />
		</tx:attributes>
	</tx:advice>
	<!-- 配置事物切入点，以及把事物和切入点属性关联起来 -->
	<aop:config>
		<aop:pointcut expression="execution(**.(..))" id="txPointCut" />
		<aop:advisor advice-ref="bookTx" pointcut-ref="txPointCut" />
	</aop:config>
```
##Spring AOP中名词的理解


- 切面(Aspect):  横切关注点(跨越应用程序多个模块的功能)被模块化的特殊对象

- 通知(Advice):  切面必须要完成的工作
也就是安全、事物、日志等。先定义好，然后再想用的地方用一下。

- 切点（pointcut）：让切点来筛选连接点，选中那几个你想要的方法 .类比：连接点相当于数据库中的记录，切点相当于查询条件。
目标(Target): 被通知的对象(主要的逻辑业务)


- 代理(Proxy): 向目标对象应用通知之后创建的对象,即实现整套AOP机制


- 连接点（Joinpoint）：就是spring允许你是通知（Advice）的地方,程序执行的某个特定位置：如类某个方法调用前、调用后、方法抛出异常后等。和方法有关的前前后后都是连接点。


- 引入（introduction） 允许我们向现有的类添加新方法属性。就是把切面（也就是新方法属性：通知定义的）用到目标类中.

所以“<aop:aspect>”实际上是定义横切逻辑，就是在连接点上做什么，“<aop:advisor>”则定义了在哪些连接点应用什么<aop:aspect>。Spring这样做的好处就是可以让多个横切逻辑（即<aop:aspect>定义的）多次使用，提供可重用性。

##AOP原理
spring用代理类包裹切面，吧他们织入到Spring管理的bean中，也就是说代理类伪装成目标类，它会截取对目标类中方法的调用，然调用者对目标类的调用都先变成伪装类，伪装类这就先执行了切面，再把调用转发给真正的目标bean。
现在可以自己想一想，怎么搞出来这个伪装类，才不会被调用者发现（过JVM的检查，JAVA是强类型检查，哪里都要检查类型）。

1. 实现和目标类相同的接口。
我也实现和你一样的接口，反正上层都是接口级别的调用，这样我就伪装成了和目标类一样的类（实现了同意接口，咱是兄弟了），也就逃过了类型检查，到java运行期的时候，利用多态的后期绑定（所以spring采用运行时），伪装类（代理类）就变成了接口的真正实现，二他里面包裹了真实的那个目标类，最后实现具体功能的还是目标类，只是不过伪装在之前干了点事情（写日志，安全检查，事物等）。
这就好比一个人让你办事，每次这个时候，你弟弟就会出来，当然他分不出来了，以为是你，你这个弟弟虽然办不了这个事，但是她知道你能办，所以就答应下来了，并且收了点礼物（写日志），收完礼物了，给把事给人家办了啊，所以你弟弟又找你这个哥哥来了，最后把这事办了还是你自己。但是你自己并不知道你弟弟已经收了礼物了，你只是专心把这件事做好。
顺着这个思想，要是本身这个类就没实现一个接口呢，你怎么伪装我，我就压给没有机会让你搞出这个双胞胎弟弟，那么就用第2种代理方式，创建一个目标类的子类，生个儿子，让儿子伪装我。

2. 生成子类调用。
这次用子类来做伪装，当然这样也能逃过JVM的强类型检查，我继承的吗，当然查不出来了，子类重写了目标类的所有方法，当然在这些重写的方法中，不仅实现了目标类的功能，还在这些功能之前，实现了一些其他的（写日志，安全检查，事物等）。
这次的对比就是，儿子先从爸爸那儿把本事都学会了，所有人都找儿子办事，但是儿子每次办和爸爸同样的事之前，都要收点小礼物（写日志），然后才去办真正的事。当然爸爸是不知道儿子这么干的了。这里就有事情要说，某些本事是爸爸独有（final的），儿子学不会，学不了就办不了这个事，办不了这个事情，自然就不能收人家的礼物了。
前一种兄弟模式，spring会使用JDK的java.lang.reflect.Proxy类，它允许Spring动态生成一个新类来实现必要的接口，织入通知，并且把这些接口的任何调用都转发到目标类。
后一种父子模式，spring使用CGLIB库生成目标类的一个子类，在创建这个子类的时候，spring织入通知，并且把对这个子类的调用委托到目标类。

相比之下，还是兄弟模式好一些，她能更好的实现松耦合，尤其在今天都高喊着面向接口编程的情况下，父子模式只是在没有实现接口的时候，也能织入通知，应该当做一种例外。
参考链接地址[http://www.hongyanliren.com/2014m12/22797.html](http://www.hongyanliren.com/2014m12/22797.html "AOP那些学术概念—通知、增强处理连接点（JoinPoint)切面（Aspect）")