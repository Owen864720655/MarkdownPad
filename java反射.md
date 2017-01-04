## java反射 ##
**反射机制**是在运行状态中，对于任意一个类，都能够知道这个类的所有属性和方法；对于任意一个对象，都能够调用它的任意一个方法和属性；这种动态获取的信息以及动态调用对象的方法的功能称为java语言的反射机制。
**反射可以完成的任务**
1. 在运行时判断任意一个对象所属的类。
2. 在运行时构造任意一个类的对象
3. 在运行时判断一个类所具有的所有成员变量和方法
4. 在运行时调用任意一个对象的方法
5. 生成动态代理


### 反射主要相关的类 ###
`Class` 类：代表一个类。
`Field` 类：代表类的成员变量（成员变量也称为类的属性）。
`Method` 类：代表类的方法。
`Constructor` 类：代表类的构造方法。
`Array` 类：提供了动态创建数组，以及访问数组的元素的静态方法。

## 核心API ##
Reflection API 中的核心类，它有以下方法
`getName()` ：获得类的完整名字。
`getFields()` ：获得类的 public 类型的属性。
`getDeclaredFields() `：获得类的所有属性。
`getMethods() `：获得类的 public 类型的方法。
`getDeclaredMethods() `：获得类的所有方法。
`getMethod(String name, Class[] parameterTypes) `：获得类的特定方法， name 参数指定方法的名字， parameterTypes 参数指定方法的参数类型。
`getConstructors()` ：获得类的 public 类型的构造方法。
`getConstructor(Class[] parameterTypes)` ：获得类的特定构造方法， parameterTypes 参数指定构造方法的参数类型。
`newInstance()` ：通过类的不带参数的构造方法创建这个类的一个对象。


### 获得某个类的全部属性 ###
```java
	Class<?> class1 = Class.forName("com.yth.test.TestReflect");
		System.out.println(class1.getName());
		Field[] fields = class1.getDeclaredFields();
		if (fields.length > 0) {
			for (Field field : fields) {
				System.out.println("修饰符" + Modifier.toString(field.getModifiers()));
				System.out.println(field.getType().getName() + "属性名" + field.getName());
			}
		}
```
获得所有的字段`Field[] fields = class1.getDeclaredFields();`
获得属性的修饰符
`Modifier.toString(field.getModifiers())`
获得属性的类型`field.getType()`返回类型是`Class`
获得属性名`field.getName()`

### 获得某个类的全部方法 ###

```java
		Class<?> class1 = Class.forName("com.yth.test.TestReflect");
		Method[] methods = class1.getDeclaredMethods();
		if (methods.length > 0) {
			for (Method method : methods) {
				Class<?> returnType = method.getReturnType();
				Class<?>[] paramType = method.getParameterTypes();
				int modiffer = method.getModifiers();
				String methodName = method.getName();
				System.out.println("---------");
				System.out.println("返回类型" + returnType.getName());
				if (paramType.length > 0) {
					for (Class x : paramType) {
						System.out.println("参数类型" + x.getName());
					}

				}else {
					System.out.println("无参数");
				}
		

				System.out.println("修饰符" + Modifier.toString(modiffer));
				System.out.println("方法名" + methodName);
				System.out.println("---------");
			}
		}
```
获得类下面所有的方法`class1.getDeclaredMethods();`
获得方法的返回值`class1.getDeclaredMethods()`返回`Class`类型
获得方法的参数`method.getParameterTypes()`返回`Class<?>[]`类型
获得方法的修饰符  和属性类似

http://baike.xsoftlab.net/view/209.html#1

### 通过反射操作某个类的方法 ###
```java
Object obj = class1.newInstance();
		method.invoke(obj);
		Method method2 = class1.getMethod("setName", String.class);
		method2.invoke(obj, "姚腾辉");
```
先通过class获取某个类的方法`getMethod`
通过`method`的`invoke`调用方法参数是，对象，和属性值列表

### 通过反射操作某个类的属性 ###
```java
Field field = class1.getDeclaredField("name");
		field.setAccessible(true);
		field.set(obj, "Java反射机制");
		System.out.println(field.get(obj));
```

设置属性的值field.set
获取属性的值field.get


动态代理的好文章http://www.cnblogs.com/flyoung2008/archive/2013/08/11/3251148.html

一个典型的动态代理创建对象过程可分为以下四个步骤：
1、通过实现InvocationHandler接口创建自己的调用处理器 IvocationHandler handler = new InvocationHandlerImpl(...);
2、通过为Proxy类指定ClassLoader对象和一组interface创建动态代理类
Class clazz = Proxy.getProxyClass(classLoader,new Class[]{...});
3、通过反射机制获取动态代理类的构造函数，其参数类型是调用处理器接口类型
Constructor constructor = clazz.getConstructor(new Class[]{InvocationHandler.class});
4、通过构造函数创建代理类实例，此时需将调用处理器对象作为参数被传入
Interface Proxy = (Interface)constructor.newInstance(new Object[] (handler));
为了简化对象创建过程，Proxy类中的newInstance方法封装了2~4，只需两步即可完成代理对象的创建。
生成的ProxySubject继承Proxy类实现Subject接口，实现的Subject的方法实际调用处理器的invoke方法，而invoke方法利用反射调用的是被代理对象的的方法（Object result=method.invoke(proxied,args)）

执行流程
首先有一个接口，一个具体执行类，实现这个接口。
然后需要一个类实现`InvocationHandler`接口。
调用时
```java
Subject proxySubject = (Subject) Proxy.newProxyInstance(Subject.class.getClassLoader(),
				new Class[] { Subject.class }, new ProxyHandler(subImp));
```
通过反射构建代理类的实例。
最终通过代理类调用接口的方法。
> 之所以通过反射来实现，是因为，我们并不知道委托类是哪一个。所以通过反射获取代理类的构造函数，然后创建代理实例。
