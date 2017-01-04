Returns an instance of a proxy class for the specified interfaces that dispatches method invocations to the specified invocation handler. 

loader the class loader to define the proxy class
interfaces the list of interfaces for the proxy class to implementh (如果有，说明我们代理类实现了它的方法，就可以通过代理类执行他的方法)
the invocation handler to dispatch method invocations to

`java.lang.reflect.InvocationHandler`
Each proxy instance has an associated invocation handler. When a method is invoked on a proxy instance, the method invocation is encoded and dispatched to the invoke method of its invocation handler.
每一个代理实例都有一个关联的调用处理器，当代理类调用一个方法时，方法调用器被解码，并且分发到它的invocation handler的invoke方法去执行。

`getAnnotations()`
Returns annotations that are present on this element. If there are no annotations present on this element, the return value is an array of length 0. The caller of this method is free to modify the returned array; it will have no effect on the arrays returned to other callers.
返回这个元素上的注解数组，如果这个元素上没有注解，则返回一个长度为0 的数组，这个方法的调用者可以自由的修改返回的数组，而不会影响其他调用者。
`getDeclaredAnnotations()`
Returns annotations that are directly present on this element. This method ignores inherited annotations. If there are no annotations directly present on this element, the return value is an array of length 0. The caller of this method is free to modify the returned array; it will have no effect on the arrays returned to other callers.
