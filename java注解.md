# java注解 #

元注解(meta-annotation):
@Target
@Retention
@Documented
@Interited
 
@Target说明注解的使用范围  `ElementType`有
　　　  1. `CONSTRUCTOR`:用于描述构造器
　　　　2. `FIELD`:用于描述域
　　　　3. `LOCAL_VARIABLE`:用于描述局部变量
　　　　4. `METHOD`:用于描述方法
　　　　5. `PACKAGE`:用于描述包
　　　　6. `PARAMETER`:用于描述参数
　　　　7. `TYPE`:用于描述类、接口(包括注解类型) 或enum声明

@Retention 定义Annotation的保留时间长短,取值（RetentionPoicy）有
1. SOURCE:在源文件中保留
2. CLASS：在class文件中有效
3. RUNTIME：在运行时有效(这样注解处理器可以通过反射，获取到该注解的属性值，从而去做一些运行时的逻辑处理)

@Documented:
用于描述其它类型的annotation应该被作为被标注的程序成员的公共API，因此可以被例如javadoc此类的工具文档化。Documented是一个标记注解，没有成员。

@Inherited
如果一个使用了@Inherited修饰的annotation类型被用于一个class，则这个annotation将被用于该class的子类。相当于被注解类的子类也添加了这个注解。(只对类有效,方法的不会被继承，接口的实现也不会继承)
子类的注解，会覆盖父类的注解。
## 自定义注解之路 ##
```java
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.FIELD)
public @interface FruitSup {
	public int id() default -1;
	public String name() default "";
	public String address() default "";
}

```
解析注解
先通过反射获取某个类的被注解的元素。然后通过下面的方法返回Annotation信息。
`getAnnotation(Class<T>annotationClass)`返回元素上是否存在某个注解。存在就返回相应的注解，不存在就返回null。获取相应的注解实例后，就可以获得注解里面的值了。
`Annotation[] getAnnotations()`返回元素上的所有注解
`boolean is AnnotationPresent(Class<?extends Annotation> annotationClass)`:判断该程序元素上是否包含指定类型的注解，存在则返回true，否则返回false.
`Annotation[] getDeclaredAnnotations()`：返回直接存在于此元素上的所有注释。该方法将忽略继承的注释。

解析方法和类上所有的注解
```java
Class<?> class1 = Chlied.class;
		Method[] methods = class1.getDeclaredMethods();
		// 遍历所有方法上的注解
		for (Method ms : methods) {
			Annotation[] annotations = ms.getAnnotations();
			for (Annotation msAn : annotations) {
				if (msAn instanceof HaHa) {
					HaHa anno = (HaHa) msAn;
					System.out.println(anno.name());
				}
				if (msAn instanceof InheritedTest) {
					InheritedTest anno = (InheritedTest) msAn;
					System.out.println(anno.name());
				}
			}
		}
Annotation[] annotations = class1.getAnnotations();
		for (Annotation as : annotations) {
			if (as instanceof HaHa) {
				HaHa aHa = (HaHa) as;
				System.out.println(aHa.name());
			}
			if (as instanceof InheritedTest) {
				InheritedTest inheritedTest = (InheritedTest) as;
				System.out.println(inheritedTest.name());
			}
		}

```
通过 `instanceof`判断属于哪个类

## 通过注解，反射的一个dao实例 ##

实体类
```java
package com.yth.dao;

@Table("user")
public class User {
	@Column("id")
	private int id;
	@Column("name")
	private String name;
	@Column("nick_name")
	private String nickName;
	@Column("age")
	private int age;
	@Column("sex")
	private String sex;
	@Column("phone_num")
	private int phoneNum;

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getNickName() {
		return nickName;
	}

	public void setNickName(String nickName) {
		this.nickName = nickName;
	}

	public int getAge() {
		return age;
	}

	public void setAge(int age) {
		this.age = age;
	}

	public String getSex() {
		return sex;
	}

	public void setSex(String sex) {
		this.sex = sex;
	}

	public int getPhoneNum() {
		return phoneNum;
	}

	public void setPhoneNum(int phoneNum) {
		this.phoneNum = phoneNum;
	}

}

```
@Table
```java
package com.yth.dao;

import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target(ElementType.TYPE)
@Retention(RetentionPolicy.RUNTIME)
@Documented
/**
 * 表名
 * @author yth
 *
 */
public @interface Table {
	public String value();
}

```

@Column
```java
package com.yth.dao;

import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target(ElementType.FIELD)
@Retention(RetentionPolicy.RUNTIME)
@Documented
/**
 * 字段名
 * @author yth
 */
public @interface Column {
	public String value();

}

```

测试类

```java
package com.yth.dao;

import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import javax.jws.soap.SOAPBinding.Use;
import javax.management.Query;

public class Test {

	public static void main(String[] args) throws Exception, IllegalArgumentException, Exception {
		User user1 = new User();
		user1.setId(1);
		user1.setName("yth");
		query(user1);
	}

	/**
	 * 查询
	 * 
	 * @param user
	 * @throws Exception
	 * @throws IllegalArgumentException
	 * @throws IllegalAccessException
	 */
	private static void query(User user) throws IllegalAccessException, IllegalArgumentException, Exception {
		StringBuffer stringBuffer = new StringBuffer();
		Class class1 = user.getClass();
		boolean isTable = class1.isAnnotationPresent(Table.class);
		// 如果被@Table注解
		if (isTable) {
			Table table = (Table) class1.getAnnotation(Table.class);
			// 表名
			String tableName = table.value();
			stringBuffer.append("select*from" + " " + tableName + " where");
			// 获取所有的字段
			Field[] fields = class1.getDeclaredFields();
			for (Field field : fields) {
				// 如果字段被Column
				boolean isExit = field.isAnnotationPresent(Column.class);
				// 如果不存在，结束本次循环，开始下一次。
				if (!isExit) {
					continue;
				}
				// 字段名
				String fieldName = field.getName();
				// 表中字段名
				String columnName = field.getAnnotation(Column.class).value();
				// 获取对象中字段的值，通过get方法。
				String getMethodName = "get" + fieldName.substring(0, 1).toUpperCase() + fieldName.substring(1);
				Method method = class1.getMethod(getMethodName);
				// 获取对象中字段值
				Object fieldValue = method.invoke(user);
	
				// 拼接sql
				if (fieldValue == null || (fieldValue instanceof Integer && (Integer) fieldValue == 0)) {
					continue;
				}
				// 如果是字符串的话需要自值两边加单引号
				if (fieldValue instanceof String) {
					stringBuffer.append(" and ").append(tableName).append(".").append(columnName).append("=")
							.append("'").append(fieldValue).append("'");
				} else {
					stringBuffer.append(" and ").append(tableName).append(".").append(columnName).append("=")
							.append(fieldValue);
				}
			}
		}
		// 去掉第一个and
		String sql = stringBuffer.toString().replaceFirst("and ", "");
		System.out.println(sql);
	}
}

```

输出
```sql
select*from user where user.id=1 and user.name='yth'
```
嘿嘿