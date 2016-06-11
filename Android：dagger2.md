 @Inject：在需要依赖的地方使用这个注解，告诉Dagger这个类或者字段需要依赖注入，这样Dagger会构造一个这个类实例来满足依赖。


 @Module：用来修饰类，表示此类的方法是用来提供依赖的，它告诉Dagger在哪里可以找到依赖。

@Provides：在@Module 中使用，我们定义的方法用这个注解，用于告诉 Dagger 我们需要构造实例并提供依赖。  默认情况下，Dagger满足依赖关系是通过调用构造方法得到的实例
构造方法有多个、三方库中的类我们不知道他是怎么实现的等等不能用 @Injec

@Singleton 单例，使用@Singleton注解之后，对象只会被初始化一次，之后的每次都会被直接注入相同的对象。@Singleton就是一个内置的作用域。

@Component： 是@Inject和@Module的桥梁,需要列出所有的@Modules以组成该组件。