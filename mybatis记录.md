## Hello World ##
## 基于XML ##
建立数据库
```sql
CREATE DATABASE mybatis01;
USE mybatis01;
CREATE TABLE user(
id INT PRIMARY KEY AUTO_INCREMENT,
name VARCHAR(20),
age INT NOT NULL
);
INSERT INTO user(name,age)VALUES("yth01",21);
INSERT INTO user(name,age)VALUES("yth02",22);
INSERT INTO user(name,age)VALUES("yth03",23);
```

建立实体
```java
package com.mybatis.module;

public class User {
	private int id;
	private String name;
	private int age;
	//getter和setter方法
	@Override
	public String toString() {
		return "User [id=" + id + ", name=" + name + ", age=" + age + "]";
	}
}

```
在实体类所在包下建立userMapper.xml
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE mapper PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN" 
"http://mybatis.org/dtd/mybatis-3-mapper.dtd">

<mapper namespace="com.mybatis.module.userMapper">
	<select id="getUserById" parameterType="int" resultType="com.mybatis.module.User">
		select *from `user` where id = #{id}
	</select>
</mapper>
```

完整的mybatis配置文件
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE configuration PUBLIC "-//mybatis.org//DTD Config 3.0//EN" 
"http://mybatis.org/dtd/mybatis-3-config.dtd">
<configuration>
	<environments default="development">
		<environment id="development">
			<transactionManager type="JDBC" />
			<dataSource type="POOLED">
				<property name="driver" value="com.mysql.jdbc.Driver" />
				<property name="url" value="jdbc:mysql://localhost:3306/mybatis01" />
				<property name="username" value="root" />
				<property name="password" value="261229" />
			</dataSource>
		</environment>
	</environments>
	<mappers>
		<mapper resource="com/mybatis/module/userMapper.xml" />
		<mapper class="com.mybatis.module.annotation.IUser" />
	</mappers>
</configuration>
```

查询测试:
```java
	String resource = "mybatis-config.xml"; 
		//加载 mybatis 的配置文件（它也加载关联的映射文件）
		Reader reader = Resources.getResourceAsReader(resource);
		//构建 sqlSession 的工厂
		SqlSessionFactory sessionFactory = new SqlSessionFactoryBuilder().build(reader);
		//创建能执行映射文件中 sql 的 sqlSession
		SqlSession session = sessionFactory.openSession();
		//映射 sql 的标识字符串
		String statement = "com.mybatis.module.userMapper"+".getUserById";
		//执行查询返回一个唯一 user 对象的 sql
		User user = session.selectOne(statement, 1);
		System.out.println(user);
```

## 基于接口注解的 ##

建立实体类，同上。
建立接口IUser,封装操作方法
```java
public interface IUser {
	 @Select("select * from user where id= #{id}")
	 public User getUserByID(int id);
}

```
mybatis-config.xml中映射(注意和基于xml的不同。分割是.而不是/。并且是class不是resource)
```xml
<mapper class="com.mybatis.module.annotation.IUser" />
```
查询
```java
	IUser mapper = session.getMapper(IUser.class);
	User user = mapper.getUserByID(2);
	System.out.println(user);
```

## mybatis中一对多，多对多 ##
http://www.2cto.com/database/201505/399017.html
有点麻烦,没成功，明天继续

### 首先是1对1. ###
sql文件
```sql
USE mybatis01;
CREATE TABLE department(
 id INT PRIMARY KEY AUTO_INCREMENT,
name VARCHAR(20) NOT NULL);

CREATE TABLE employee(
id INT PRIMARY KEY AUTO_INCREMENT,
name VARCHAR(20),
depid INT,
CONSTRAINT fk_dep FOREIGN KEY(depid) REFERENCES department(id)
);
INSERT INTO department(name)VALUES("部门1");
INSERT INTO department(name)VALUES("部门2");
INSERT INTO department(name)VALUES("部门3");
INSERT INTO department(name)VALUES("部门4");

INSERT INTO employee(name,depid)VALUES("员工1",1);
INSERT INTO employee(name,depid)VALUES("员工2",1);
INSERT INTO employee(name,depid)VALUES("员工3",2);
INSERT INTO employee(name,depid)VALUES("员工4",3);
```

实体类`Department`
```java
/**
 * 部门
 * @author yth
 */
public class Department {
	private int id;
	private String name;
//getter  setter方法
}
```
实体类 `Employee`
```java
/**
 * 员工
 * 
 * @author yth
 *
 */
public class Employee {
	private int id;
	private int name;
	private int depId;
	private Department department;
}
```
配置`depMapper.xml`
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE mapper PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN" 
"http://mybatis.org/dtd/mybatis-3-mapper.dtd">
<!-- SELECT *FROM manager m,department d WHERE m.dep_id=d.id AND d.id=1; -->
<mapper namespace="com.yth.test2.depMapper">
	<select id="getDep" parameterType="int" resultMap="DepMap">
		SELECT *FROM
		manager m,department d WHERE m.dep_id=d.id AND m.id=#{id}
	</select>
	<resultMap type="Manager" id="DepMap">
		<id property="id" column="id" />
		<result property="name" column="name" />
		<association property="department" column="dep_id" javaType="Department">
			<id property="id" column="id" />
			<result property="name" column="name" />
		</association>
	</resultMap>
</mapper>
```
在mybatis-config.xml中加入`<mapper resource="com/yth/test2/depMapper.xml" />`

> 注意:刚开始不成功`depMapper.xml`文件`resultMap`标签中类名写成全类名 或者在mybatis-config.xml中配置 别名。

	<typeAliases>
		<package name="com.mybatis.module"/>
	</typeAliases>

### 然后一对多 ###
首先是1端配置。1对多查询。
数据库文件
```sql
USE mybatis01;
CREATE TABLE person(
p_id INT PRIMARY KEY AUTO_INCREMENT,
name VARCHAR(20)
);
CREATE TABLE orders(
o_id INT PRIMARY KEY AUTO_INCREMENT,


price INT,
pid INT 
-- CONSTRAINT fk_person FOREIGN KEY(pid) REFERENCES person(p_id)
-- CONSTRAINT fk_dep FOREIGN KEY(pid) REFERENCES person(p_id)
);
ALTER TABLE orders ADD CONSTRAINT fk_person FOREIGN KEY (pid) REFERENCES 
person(p_id);


INSERT INTO person(name)VALUES("姚腾辉");
INSERT INTO person(name)VALUES("姚腾辉2");
INSERT INTO orders(pid,price)VALUES(1,12);
INSERT INTO orders(pid,price)VALUES(1,14);
INSERT INTO orders(pid,price)VALUES(2,3);
INSERT INTO orders(pid,price)VALUES(3,13);
```

实体类
Person
```java
public class Person {
	private int id;  
    private String name;  
    private List<Orders> orderList;    
}

```
Orders
```java
package com.mybatis.module;

public class Orders {
    private int id;  
    private double price;  
    private int pid;
    private Person person;
}

```
一对多配置
PerdonMapper.xml
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE mapper
  PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN"
  "http://mybatis.org/dtd/mybatis-3-mapper.dtd">

<mapper namespace="com.yth.test4.PersonMapper">
	<select id="selectPersonFetchOrder" parameterType="int"
		resultMap="personreSultMap">
		select p.*,o.* from person p,orders o
		where o.pid=p.p_id and
		p.p_id=#{id}
	</select>
	<resultMap type="Person" id="personreSultMap">
		<id property="id" column="p_id" />
		<result property="name" column="name" />
		<collection property="orderList" ofType="Orders" column="pid">
			<id property="id" column="o_id" />
			<result property="price" column="price" />
			<result property="pid" column="pid" />
		</collection>
	</resultMap>
</mapper>
```
在mybatis-config.xml中加入mapper文件
```xml
<mapper resource="com/yth/test4/PersonMapper.xml" />
```

多对一的配置
OrdersMapper.xml
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE mapper
  PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN"
  "http://mybatis.org/dtd/mybatis-3-mapper.dtd">

<mapper namespace="com.yth.test4.OrderMapper">
	<select id="selectOrderFetchOrder" parameterType="int"
		resultMap="orderreSultMap">
		select p.*,o.* from person p,orders o
		where o.pid=p.p_id and
		o.o_id=#{id}
	</select>
	<resultMap type="Orders" id="orderreSultMap">
		<id property="id" column="o_id" />
		<result property="price" column="price" />
		<result property="pid" column="pid" />
		<association property="person" javaType="Person">
			<id property="id" column="p_id" />
			<result property="name" column="name" />
		</association>
	</resultMap>

</mapper>
```

在mybatis-config.xml中加入mapper文件
```xml
<mapper resource="com/yth/test4/OrdersMapper.xml" />
```

## MyBatis的动态SQL详解 ##
文章地址http://elim.iteye.com/blog/1338557
**if:**其实就是`WHERE`后面条件的简单化处理
`Blog`是条件实体(条件实体可以和查询实体一样。)，`if`标签中`test`是条件。
```xml
<select id="dynamicIfTest" parameterType="Blog" resultType="Blog">  
    select * from t_blog where 11 = 1  
    <if test="title != null">  
        and title = #{title}  
    </if>  
    <if test="content != null">  
        and content = #{content}  
    </if>  
    <if test="owner != null">  
        and owner = #{owner}  
    </if>  
</select> 
```
如果提供了title参数，那么就要满足`title=#{title}`，同样如果你提供了`Content`和`Owner`的时候，它们也需要满足相应的条件，之后就是返回满足这些条件的所有Blog

**choose元素** 元素的作用就相当于JAVA中的switch语句
```xml
<select id="dynamicChooseTest" parameterType="Blog" resultType="Blog">  
    select * from t_blog where 11 = 1   
    <choose>  
        <when test="title != null">  
            and title = #{title}  
        </when>  
        <when test="content != null">  
            and content = #{content}  
        </when>  
        <otherwise>  
            and owner = "owner1"  
        </otherwise>  
    </choose>  
```
when元素表示当when中的条件满足的时候就输出其中的内容，跟JAVA中的switch效果差不多的是按照条件的顺序，当when中有条件满足的时候，就会跳出choose，即所有的when和otherwise条件中，只有一个会输出，当所有的我很条件都不满足的时候就输出otherwise中的内容。

**where语句** 的作用主要是简化SQL语句中where中的条件判断的
```xml
<select id="dynamicWhereTest" parameterType="Blog" resultType="Blog">  
    select * from t_blog   
    <where>  
        <if test="title != null">  
            title = #{title}  
        </if>  
        <if test="content != null">  
            and content = #{content}  
        </if>  
        <if test="owner != null">  
            and owner = #{owner}  
        </if>  
    </where>  
</select>  
```
where元素的作用是会在写入where元素的地方输出一个where，另外一个好处是你不需要考虑where元素里面的条件输出是什么样子的，MyBatis会智能的帮你处理，如果所有的条件都不满足那么MyBatis就会查出所有的记录，如果输出后是and 开头的，MyBatis会把第一个and忽略，当然如果是or开头的，MyBatis也会把它忽略。
**set元素**
主要是用在更新操作的时候，它的主要功能和where元素其实是差不多的，主要是在包含的语句前输出一个set，然后如果包含的语句是以逗号结束的话将会把该逗号忽略，如果set包含的内容为空的话则会出错。
```xml
<update id="dynamicSetTest" parameterType="Blog">  
    update t_blog  
    <set>  
        <if test="title != null">  
            title = #{title},  
        </if>  
        <if test="content != null">  
            content = #{content},  
        </if>  
        <if test="owner != null">  
            owner = #{owner}  
        </if>  
    </set>  
    where id = #{id}  
</update>  
```
set中一个条件都不满足，即set中包含的内容为空的时候就会报错。
测试是通过的，注意，`if`标签中的","同理where标签中也注意开头的 and/or.


**trim标记**相当于一个外能标记
```xml
　　select * from user 
　　<trim prefix="WHERE" prefixoverride="AND |OR">
　　　　<if test="name != null and name.length()>0"> AND name=#{name}</if>
　　　　<if test="gender != null and gender.length()>0"> AND gender=#{gender}</if>
　　</trim>
```
`prefix`前缀
`suffix`后缀
`prefixoverride`去掉第and/or(相对上文来说)
`suffixoverride`去掉最优一个标记

可以把trim当做一个字符串拼接。上面的标签都是相对于 拼接的整个字符串来说的


## 框架整合 ##
### spring整合mybatis ###

基于maven
项目目录结构
![](http://i.imgur.com/jT6BTwV.png)
建立数据库
```sql
USE mybatismvc;
CREATE TABLE s_user(
user_id INT AUTO_INCREMENT PRIMARY KEY,
user_name VARCHAR(30),
user_birthday DATE,
user_salary DOUBLE
)
```

建立实体类
```java
package com.atguigu.day03_ms.bean;
import java.util.Date;
public class User {
	private int id;
	private String name;
	private Date birthday;
	private double salary;
}

```

建立数据库操作的接口和mapper映射文件
```java
package com.atguigu.day03_ms.dao;

import java.util.List;

import com.atguigu.day03_ms.bean.User;

public interface UserMapper {

	void save(User user);

	void update(User user);

	void delete(int id);

	User findById(int id);

	List<User> findAll();

}

```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE mapper PUBLIC "-//mybatis.org//DTD Mapper 3.0//EN" 
"http://mybatis.org/dtd/mybatis-3-mapper.dtd">
<!--namespace必须是接口的全类名  -->
<mapper namespace="com.atguigu.day03_ms.dao.UserMapper">
	
	<resultMap type="User" id="userResult">
		<result column="user_id" property="id"/>
		<result column="user_name" property="name"/>
		<result column="user_birthday" property="birthday"/>
		<result column="user_salary" property="salary"/>
	</resultMap>

	<!-- 取得插入数据后的id -->
	<insert id="save" keyColumn="user_id" keyProperty="id" useGeneratedKeys="true">
		insert into s_user(user_name,user_birthday,user_salary)
		values(#{name},#{birthday},#{salary})
	</insert>

	<update id="update">
		update s_user
		set user_name = #{name},
			user_birthday = #{birthday},
			user_salary = #{salary}
		where user_id = #{id}
	</update>
	
	<delete id="delete">
		delete from s_user
		where user_id = #{id}
	</delete>

	<select id="findById" resultMap="userResult">
		select *
		from s_user
		where user_id = #{id}
	</select>
	
	<select id="findAll" resultMap="userResult">
		select * 
		from s_user
	</select>
</mapper>

```

建立spring-mybatis配置文件
```xml
<?xml version="1.0" encoding="UTF-8"?>
<beans xmlns="http://www.springframework.org/schema/beans"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:p="http://www.springframework.org/schema/p"
	xmlns:context="http://www.springframework.org/schema/context" xmlns:tx="http://www.springframework.org/schema/tx"
	xsi:schemaLocation="
		http://www.springframework.org/schema/beans
		http://www.springframework.org/schema/beans/spring-beans-3.2.xsd
		http://www.springframework.org/schema/context
		http://www.springframework.org/schema/context/spring-context-3.2.xsd
		http://www.springframework.org/schema/tx
		http://www.springframework.org/schema/tx/spring-tx-3.2.xsd">
	<!-- 1. 数据源 : DriverManagerDataSource -->
	<context:component-scan base-package="com.atguigu.day03_ms"></context:component-scan>
	<bean id="datasource"
		class="org.springframework.jdbc.datasource.DriverManagerDataSource">
		<property name="driverClassName" value="com.mysql.jdbc.Driver" />
		<property name="url" value="jdbc:mysql://localhost:3306/mybatismvc" />
		<property name="username" value="root" />
		<property name="password" value="261229" />
	</bean>
	<!-- 2. mybatis的SqlSession的工厂: SqlSessionFactoryBean dataSource / typeAliasesPackage -->
	<bean id="sqlSessionFactory" class="org.mybatis.spring.SqlSessionFactoryBean">
		<property name="dataSource" ref="datasource" />
		<property name="mapperLocations" value="classpath:com/atguigu/day03_ms/dao/mapper/*.xml"/>
		<property name="typeAliasesPackage" value="com.atguigu.day03_ms.bean" />
	</bean>
	<!-- 3. mybatis自动扫描加载Sql映射文件 : MapperScannerConfigurer sqlSessionFactory 
		/ basePackage -->
	<bean id="config" class="org.mybatis.spring.mapper.MapperScannerConfigurer">
		<property name="basePackage" value="com.atguigu.day03_ms.dao" />
		<property name="sqlSessionFactory" ref="sqlSessionFactory" />
	</bean>
	<!-- 4. 事务管理 : DataSourceTransactionManager -->
	<bean id="manager"
		class="org.springframework.jdbc.datasource.DataSourceTransactionManager">
		<property name="dataSource" ref="datasource" />
	</bean>
	<!-- 5. 使用声明式事务 -->
	<tx:annotation-driven transaction-manager="manager" />
</beans>

```
- Mybatis-Spring给我们封装了一个`SqlSessionFactoryBean`，在这个bean里面还是通过`SqlSessionFactoryBuilder`来建立对应的`SqlSessionFactory`，进而获取到对应的`SqlSession`。
- `mapperLocations`：它表示我们的Mapper文件存放的位置，当我们的Mapper文件跟对应的Mapper 接口处于同一位置的时候可以**不用指定该属性的值**。
- `configLocation`：用于指定Mybatis的配置文件位置。如果指定了该属性，那么会以该配置文件的内容作为配置信息构建对应的SqlSessionFactoryBuilder，但是后续属性指定的内容会覆盖该配置文件里面指定的对应内容。(我们这里不需要这个配置文件，对mybatis的配置全部在spring中完成)
- typeAliasesPackage：它一般对应我们的实体类所在的包，这个时候会自动取对应包中不包括包名的简单类名作为包括包名的别名。多个package之间可以用逗号或者分号等来进行分隔。
- `MapperScannerConfigurer`如果我们需要使用MapperScannerConfigurer来帮我们自动扫描和注册Mapper接口的话我们需要在Spring的applicationContext配置文件中定义一个MapperScannerConfigurer对应的bean。对于MapperScannerConfigurer而言有一个属性是我们必须指定的，那就是basePackage。basePackage是用来指定Mapper接口文件所在的基包的，在这个基包或其所有子包下面的Mapper接口都将被搜索到

这篇文章还不错http://www.tuicool.com/articles/FVRzI3。

建立Service接口
```java
package com.atguigu.day03_ms.service;
import com.atguigu.day03_ms.bean.User;
public interface IUserService {
	public User getUserById(int userId);
}

```
实现类
```java
package com.atguigu.day03_ms.service.impl;
import javax.annotation.Resource;
import org.springframework.stereotype.Service;
import com.atguigu.day03_ms.bean.User;
import com.atguigu.day03_ms.dao.UserMapper;
import com.atguigu.day03_ms.service.IUserService;
@Service("userService")
public class UserServiceImpl implements IUserService {
	@Resource
	private UserMapper userMapper;
	@Override
	public User getUserById(int userId) {
		return this.userMapper.findById(userId);
	}
}

```

测试类
```java
package mybatismvc;

import org.apache.log4j.Logger;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
import com.atguigu.day03_ms.bean.User;
import com.atguigu.day03_ms.dao.UserMapper;
import com.atguigu.day03_ms.service.IUserService;
@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration("/beans.xml")
public class TestMybatis {
	private ApplicationContext ac = null;
	private static Logger logger = Logger.getLogger(TestMybatis.class);
	@Autowired
	private IUserService userService;
	@Test
	public void testAdd() {
		User user = userService.getUserById(1);
		System.out.println(user);
	}
}
```

pom.xml
```xml
<project xmlns="http://maven.apache.org/POM/4.0.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd">
	<modelVersion>4.0.0</modelVersion>
	<groupId>com.yth</groupId>
	<artifactId>mybatismvc</artifactId>
	<packaging>war</packaging>
	<version>0.0.1-SNAPSHOT</version>
	<name>mybatismvc Maven Webapp</name>
	<url>http://maven.apache.org</url>
	<properties>
		<!-- spring版本号 -->
		<spring.version>4.0.2.RELEASE</spring.version>
		<!-- mybatis版本号 -->
		<mybatis.version>3.2.6</mybatis.version>
		<!-- log4j日志文件管理包版本 -->
		<slf4j.version>1.7.7</slf4j.version>
		<log4j.version>1.2.17</log4j.version>
	</properties>

	<dependencies>
		<dependency>
			<groupId>junit</groupId>
			<artifactId>junit</artifactId>
			<version>3.8.2</version>
			<!-- 表示开发的时候引入，发布的时候不会加载此包 -->
			<scope>test</scope>
		</dependency>
		<!-- spring核心包 -->
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-core</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-web</artifactId>
			<version>${spring.version}</version>
		</dependency>
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-oxm</artifactId>
			<version>${spring.version}</version>
		</dependency>
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-tx</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-jdbc</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-webmvc</artifactId>
			<version>${spring.version}</version>
		</dependency>
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-aop</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-context-support</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-test</artifactId>
			<version>${spring.version}</version>
		</dependency>
		<!-- mybatis核心包 -->
		<dependency>
			<groupId>org.mybatis</groupId>
			<artifactId>mybatis</artifactId>
			<version>${mybatis.version}</version>
		</dependency>
		<!-- mybatis/spring包 -->
		<dependency>
			<groupId>org.mybatis</groupId>
			<artifactId>mybatis-spring</artifactId>
			<version>1.2.2</version>
		</dependency>
		<!-- 导入java ee jar 包 -->
		<dependency>
			<groupId>javax</groupId>
			<artifactId>javaee-api</artifactId>
			<version>7.0</version>
		</dependency>
		<!-- 导入Mysql数据库链接jar包 -->
		<dependency>
			<groupId>mysql</groupId>
			<artifactId>mysql-connector-java</artifactId>
			<version>5.1.38</version>
		</dependency>
		<!-- 导入dbcp的jar包，用来在applicationContext.xml中配置数据库 -->
		<dependency>
			<groupId>commons-dbcp</groupId>
			<artifactId>commons-dbcp</artifactId>
			<version>1.2.2</version>
		</dependency>
		<!-- JSTL标签类 -->
		<dependency>
			<groupId>jstl</groupId>
			<artifactId>jstl</artifactId>
			<version>1.2</version>
		</dependency>
		<!-- 日志文件管理包 -->
		<!-- log start -->
		<!-- <dependency> <groupId>log4j</groupId> <artifactId>log4j</artifactId> 
			<version>${log4j.version}</version> </dependency> -->


		<!-- 格式化对象，方便输出日志 -->
		<dependency>
			<groupId>com.alibaba</groupId>
			<artifactId>fastjson</artifactId>
			<version>1.1.41</version>
		</dependency>

		<dependency>
			<groupId>org.slf4j</groupId>
			<artifactId>slf4j-api</artifactId>
			<version>${slf4j.version}</version>
		</dependency>

		<dependency>
			<groupId>org.slf4j</groupId>
			<artifactId>slf4j-log4j12</artifactId>
			<version>${slf4j.version}</version>
		</dependency>
		<!-- log end -->
		<!-- 映入JSON -->
		<dependency>
			<groupId>org.codehaus.jackson</groupId>
			<artifactId>jackson-mapper-asl</artifactId>
			<version>1.9.13</version>
		</dependency>
		<!-- 上传组件包 -->
		<dependency>
			<groupId>commons-fileupload</groupId>
			<artifactId>commons-fileupload</artifactId>
			<version>1.3.1</version>
		</dependency>
		<dependency>
			<groupId>commons-io</groupId>
			<artifactId>commons-io</artifactId>
			<version>2.4</version>
		</dependency>
		<dependency>
			<groupId>commons-codec</groupId>
			<artifactId>commons-codec</artifactId>
			<version>1.9</version>
		</dependency>
	</dependencies>
	<build>
		<finalName>mybatismvc</finalName>

		<plugins>
			<plugin>
				<groupId>org.apache.maven.plugins</groupId>
				<artifactId>maven-compiler-plugin</artifactId>
				<configuration>
					<source>1.8</source>
					<target>1.8</target>
				</configuration>
			</plugin>
		</plugins>
	</build>
</project>

```