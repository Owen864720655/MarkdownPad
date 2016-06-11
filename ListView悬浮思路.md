## ListView悬浮思路 ##

> 布局其实就是浮动部分的隐藏显示

```xml
<?xml version="1.0" encoding="utf-8"?>
<RelativeLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent" >

    <TextView
        android:id="@+id/title"
        android:layout_width="match_parent"
        android:layout_height="30dp"
        android:background="#332b3b"
        android:textColor="#ffffff"
        android:gravity="center"
        android:text="标题" />

    <FrameLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_below="@id/title" >

        <ListView
            android:id="@+id/lv"
            android:layout_width="match_parent"
            android:layout_height="match_parent" />
         <LinearLayout
            android:id="@+id/invis"
            android:layout_width="fill_parent"
            android:layout_height="50dp"
            android:background="#ccedc7"
            android:orientation="horizontal"
            android:visibility="gone" >

            <TextView
                android:id="@+id/tv"
                android:layout_width="match_parent"
                android:layout_height="50dp"
                android:gravity="center"
                android:text="悬浮部分" />
        </LinearLayout>
    </FrameLayout>

</RelativeLayout>
```
>逻辑实现关机部分 
>监听滚动事件。控制浮动部分的显示隐藏
>firstVisibleItem表示课件的数量。

```java
		lv.setOnScrollListener(new OnScrollListener() {

			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {

			}

			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {
				if (firstVisibleItem >= 1) {
					invis.setVisibility(View.VISIBLE);
				} else {

					invis.setVisibility(View.GONE);
				}
			}
		});
```


关于Android 嵌套滑动机制（NestedScrolling）https://segmentfault.com/a/1190000002873657

## CoordinatorLayout学习总结 ##
### CoordinatorLayout与AppBarLayout ###
> 扩展伸缩
> 注意：必须使用一个容器布局：AppBarLayout来让Toolbar响应滚动事件。并且
 AppBarLayout目前必须是第一个嵌套在CoordinatorLayout里面的子view

> 定义AppBarLayout与滚动视图之间的联系。在RecyclerView或者任意支持嵌套滚动的view比如NestedScrollView上添加`app:layout_behavior`。当CoordinatorLayout发现RecyclerView中定义了这个属性，它会搜索自己所包含的其他view，看看是否有view与这个behavior相关联。

```xml
<?xml version="1.0" encoding="utf-8"?>
<android.support.design.widget.CoordinatorLayout
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    xmlns:tools="http://schemas.android.com/tools"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <android.support.design.widget.AppBarLayout
        android:id="@+id/appbar"
        android:layout_width="match_parent"
        android:layout_height="wrap_content">

        <android.support.v7.widget.Toolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            android:background="?attr/colorPrimary"
            app:layout_scrollFlags="scroll|enterAlways"
            app:popupTheme="@style/ThemeOverlay.AppCompat.Light"
            />
    </android.support.design.widget.AppBarLayout>

    <android.support.v7.widget.RecyclerView
        android:id="@+id/recycleview"
        app:layout_behavior="@string/appbar_scrolling_view_behavior"
        android:layout_width="match_parent"
        android:layout_height="wrap_content">
    </android.support.v7.widget.RecyclerView>
</android.support.design.widget.CoordinatorLayout>

```

## AppBarLayout嵌套CollapsingToolbarLayout ##
>布局

```xml
<?xml version="1.0" encoding="utf-8"?>
<android.support.design.widget.CoordinatorLayout
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    xmlns:tools="http://schemas.android.com/tools"
    android:layout_width="match_parent"
    android:layout_height="match_parent">
    <android.support.design.widget.AppBarLayout
        android:id="@+id/appbar"
        android:layout_width="match_parent"
        android:layout_height="280dp"
        android:fitsSystemWindows="true"
        android:theme="@style/ThemeOverlay.AppCompat.Dark.ActionBar">

        <android.support.design.widget.CollapsingToolbarLayout
            android:id="@+id/colla"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            android:fitsSystemWindows="true"
            app:contentScrim="?attr/colorPrimary"
            app:expandedTitleMarginEnd="64dp"
            app:expandedTitleMarginStart="48dp"
            app:layout_scrollFlags="scroll|exitUntilCollapsed">

            <ImageView
                android:id="@+id/backdrop"
                android:layout_width="match_parent"
                android:layout_height="match_parent"
                android:fitsSystemWindows="true"
                android:scaleType="centerCrop"
                android:src="@drawable/car"
                app:layout_collapseMode="parallax"
                />
            <android.support.v7.widget.Toolbar
                android:id="@+id/toolbar"
                android:layout_width="match_parent"
                android:layout_height="?attr/actionBarSize"
                app:layout_collapseMode="pin"
                app:popupTheme="@style/ThemeOverlay.AppCompat.Light"/>
        </android.support.design.widget.CollapsingToolbarLayout>
    </android.support.design.widget.AppBarLayout>

    <android.support.v7.widget.RecyclerView
        android:id="@+id/recycleview"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">
    </android.support.v7.widget.RecyclerView>
    <android.support.design.widget.FloatingActionButton
        android:layout_height="wrap_content"
        android:layout_width="wrap_content"
        app:layout_anchor="@id/appbar"
        app:layout_anchorGravity="bottom|right|end"
        android:src="@drawable/ic_close_black_24dp"
        android:layout_margin="16dp"
        android:clickable="true"/>

</android.support.design.widget.CoordinatorLayout>
```

>注意:

- 关于ToolBar:所在的Activity主题必须是NoActionBar
```xml
    <style name="AppTheme.NoActionBar">
        <item name="windowActionBar">false</item>
        <item name="windowNoTitle">true</item>
    </style>
```
5.0中
```xml
    <style name="AppTheme.NoActionBar">
        <item name="windowActionBar">false</item>
        <item name="windowNoTitle">true</item>
        <item name="android:windowDrawsSystemBarBackgrounds">true</item>
        <item name="android:statusBarColor">@android:color/transparent</item>
    </style>
```
-  app:contentScrim="?attr/colorPrimary" ,CollapsingToolbarLayout这个属性是设置折叠后Toolbar的颜色
-  app:layout_scrollFlags="scroll|exitUntilCollapsed"
	-  scroll: 所有想滚动出屏幕的view都需要设置这个flag， 没有设置这个flag的view将被固定在屏幕顶部
	-  enterAlways: 设置这个flag时，向下的滚动都会导致该view变为可见，启用快速“返回模式”
	-  enterAlwaysCollapsed: 当你的视图已经设置minHeight属性又使用此标志时，你的视图只能已最小高度进入，只有当滚动视图到达顶部时才扩大到完整高度。
	-  exitUntilCollapsed: 滚动退出屏幕，最后折叠在顶端。
- app:layout_collapseMode="parallax", 这是控制滚出屏幕范围的效果的
	- pin 固定模式，在折叠的时候最后固定在顶端表示不会被滚出屏幕范围
	- parallax 视差模式，在折叠的时候会有个视差折叠的效果
		- layout_collapseParallaxMultiplier(视差因子) - 设置视差滚动因子，值为：0~1。
- CoordinatorLayout 还提供了一个 layout_anchor 的属性，连同 layout_anchorGravity 一起，可以用来放置与其他视图关联在一起的悬浮视图（如 FloatingActionButton）
```xml
app:layout_anchor="@id/appbar"
app:layout_anchorGravity="bottom|right|end"
```
- CoordinatorLayout包含的子视图中带有滚动属性的View需要设置app:layout_behavior属性
  - app:layout_behavior="@string/appbar_scrolling_view_behavior"视差
  - app:layout_behavior="@string/appbar_scrolling_view_behavior"折叠

## CoordinatorLayout中Behavior自定义 ##
>在自定义Behavior的时候，需要关心的两组四个方法
>1. 某个view监听另一个view的状态变化，例如大小、位置、显示状态等
>2. 某个view监听CoordinatorLayout里的滑动状态


**layoutDependsOn**
```java
    @Override
    public boolean layoutDependsOn(CoordinatorLayout parent, View child, View dependency) {
        return super.layoutDependsOn(parent, child, dependency);
    }
```
第一个参数是当前的CoordinatorLayout第二个参数是我们设置这个Behavior的View，第三个是我们关心的那个View。
如何知道关心哪一个View？看返回值(这里关心一下TextView)
```java
return dependency instanceof TextView;
```
接下来就是在这个View状态发生变化的时候，我们现在的View该做些什么了。
**onDependentViewChanged**
```java
@Override
public boolean onDependentViewChanged(CoordinatorLayout parent, View child, View dependency) {
    int offset = dependency.getTop() - child.getTop();
    ViewCompat.offsetTopAndBottom(child, offset);
    return true;
}
```

//明天看--------------------------------------------
Android 优化交互 —— CoordinatorLayout 与 Behavior https://segmentfault.com/a/1190000005024216 
CoordinatorLayout高级用法-自定义Behavior http://www.07net01.com/2015/12/1017279.html
自定义CoordinatorLayout的Behavior实现知乎和简书快速返回效果 http://blog.csdn.net/tiankong1206/article/details/48394393