#Spring 连接 Hibernate#
##思路##
- 让IoC容器来管理Hibernate的SessionFactory
- 让Hibernate使用Spring的声明式事务
##步骤##
1. 先加入Hibernate
	1. 添加hibernate.cfg.xml
	2. 
```xml
<!-- 配置hibernate的基本属性，方言，SQL格式化，生成数据表的策略以及二级缓存 有关数据库连接的在Spring的配置文件中通过dataSource来配置-->
		<property name="dialect">org.hibernate.dialect.MySQLDialect</property>
		<!-- 是否在控制台打印SQL语句 -->
		<property name="show_sql">true</property>
		<!-- 格式化sql语句 -->
		<property name="format_sql">true</property>
		<!-- 生成数据表的策略 -->
		<property name="hbm2ddl.auto">update</property>
		<!-- 如果设置hibernate.use_identifier_rollback为true,delete()方法会把持久化对象或游离对象的OID置为null,使它们转变为临时对象,这样程序就可以重复使用这些临时对象了. -->
		<property name="use_identifier_rollback">true</property>
	```
	2. 编写持久化类并生成.hbm.xml文件
2. 加入Spring配置文件
	1. 配置数据源通过c3p0.
	2. 配置sessionFactory通过```LocalSessionFactoryBean```
	```xml
<bean id="sessionFactory"
		class="org.springframework.orm.hibernate4.LocalSessionFactoryBean">
		<!-- 配置hibernate的位置 -->
		<property name="configLocation" value="classpath:hibernate.cfg.xml"></property>
		<!-- 配置数据源 -->
		<property name="dataSource" ref="dataSource"></property>
		<!-- 配置映射文件可以使用通配符 -->
		<property name="mappingLocations" value="classpath:com/yth/book/*.hbm.xml"></property>
	</bean>
	```
	3. 配置声明式事务