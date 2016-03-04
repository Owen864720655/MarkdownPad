# RxJava #
##1.API 介绍和原理简析##
###观察者模式###
A 对象（观察者）对 B 对象（被观察者）的某种变化高度敏感，需要在 B 变化的一瞬间做出反应。![](http://i.imgur.com/BI6kH4T.png)
其中Button/View为被监听者OnclickListener为监听者 
![](http://i.imgur.com/CuHQwHK.png)

----------

RxJava 有四个基本概念
- Observable (可观察者，即被观察者)
- Observer (观察者)
- subscribe (订阅)
- 事件
