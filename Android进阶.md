#Android 充电记录#
#Android Design Support Library使用详解#
使用依赖`compile 'com.android.support:design:22.2.0'`

参考链接[http://blog.163.com/shexinyang@126/blog/static/13673931220155523634778/](http://blog.163.com/shexinyang@126/blog/static/13673931220155523634778/)
##TextInputLayout##

**TextInputLayout作为一个父容器控件**，包装了新的EditText提示信息会变成一个显示在EditText之上的floating label，这样用户就始终知道他们现在输入的是什么。同时，如果给EditText增加监听，还可以给它增加更多的floating label。
使用

```xml

 <android.support.design.widget.TextInputLayout
        android:id="@+id/til_pwd"
        android:layout_width="match_parent"
        android:hint="password"
        android:layout_height="wrap_content">
        <EditText
            android:layout_width="match_parent"
            android:layout_height="wrap_content" />
    </android.support.design.widget.TextInputLayout>

```
这里需要注意的是，TextInputLayout的颜色来自style中的colorAccent的颜色：
```xml
<item name="colorAccent">#1743b7</item>
```
监听函数

```java
        et_til.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });

```
![TextWatcher监听函数](http://i.imgur.com/auxQTif.png)

----------

    onTextChanged
这个方法是通知你当输入字符串s的时候，从start开始的count个字符替换了长度为before的字符。不能再这个回调函数中更改s的值。

理解：当输入时,count为增加的字符，before为0
当删除时,count为0,before为1（每次删除一个）
当前字符串长度总是等于start+count

----------

    beforeTextChanged
这个方法是通知你当输入字符串s的时候，从start开始的count个字符将要被长度为after的字符替换。不能再这个回调函数中更改s的值。
理解：
当输入时after为输入字符的长度count为0
当删除时，count为1，after为0
start为输入前字符串长度
当前字符串长度总是等于start+after
##Floating Action Button
FloatingActionButton继承自ImageView，可以使用android:src或者ImageView的任意方法，比如setImageDrawable()来设置FloatingActionButton里面的图标。
```xml
 <android.support.design.widget.FloatingActionButton
        android:id="@+id/fab"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="bottom|end"
        android:layout_margin="@dimen/fab_margin"
        android:src="@android:drawable/ic_dialog_email" />
```
这样就在右下角
重要属性
- app:layout_anchor - 设置FAB的锚点，即以哪个控件为参照点设置位置。
- app:layout_anchorGravity - 设置FAB相对锚点的位置，值有 bottom、center、right、left、top等。
监听函数像Button一样
```java
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
               //监听事件
            }
        });
```
##Snackbar##
snackbar和Toast比较相似显示在屏幕的底部，包含了文字信息与一个可选的操作按钮。在指定时间结束之后自动消失。另外，用户还可以在超时之前将它滑动删除。
```java
           Snackbar.make(view, "Snackbar comes out", Snackbar.LENGTH_LONG)
				.setAction("Action2", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Toast.makeText(SnackbarActivity.this, "Toast comes out2", Toast.LENGTH_SHORT).show();
                    }
                })
                .setActionTextColor(R.color.fab_stroke_end_inner_color)
                .setDuration(Snackbar.LENGTH_SHORT)
                .show();
```
##TabLayout+Viewpager##
ViewPager的用法，需要一个ViewPager的适配器。
```java
public class TabViewFragment extends FragmentPagerAdapter {
    private List<Fragment> list_fragment;                         //fragment列表
    private List<String> list_Title;                              //tab名的列表

    public TabViewFragment(FragmentManager fm, List<Fragment> list_fragment, List<String> list_Title) {
        super(fm);
        this.list_fragment = list_fragment;
        this.list_Title = list_Title;
    }

    @Override
    public Fragment getItem(int position) {
        return list_fragment.get(position);
    }

    @Override
    public int getCount() {
        return list_Title.size();
    }

    @Override
    public CharSequence getPageTitle(int position) {
        return list_Title.get(position);
    }
}
```
对于TabLayout
1. 添加Tab选项卡
```java
tableLayout.addTab(tableLayout.newTab().setText("Tab 1"));
tableLayout.addTab(tableLayout.newTab().setText("Tab 2"));
tableLayout.addTab(tableLayout.newTab().setText("Tab 3"));
```
2. 结合ViewPager步骤
```java
        list_title = new ArrayList<>();
        list_title.add("Tab1");
        list_title.add("Tab2");
        list_title.add("Tab3");

        tab1Frag = new Tab1();
        tab2Frag = new Tab2();
        tab3Frag = new Tab3();

        list_fragment = new ArrayList<>();
        list_fragment.add(tab1Frag);
        list_fragment.add(tab2Frag);
        list_fragment.add(tab3Frag);

//设置tab模式
        tableLayout.setTabMode(TabLayout.MODE_FIXED);
//创建FragmentPagerAdaper
        tabViewFragment = new TabViewFragment(getSupportFragmentManager(), list_fragment, list_title);
//ViewPager设置Adapter
        vpFindFragmentPager.setAdapter(tabViewFragment);
//tabLayout设置viewPager
        tableLayout.setupWithViewPager(vpFindFragmentPager);
```
##ToolBar##
首先Activity的主题需要设置成NoActionBar
在布局文件中
```xml
        <android.support.v7.widget.Toolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            android:background="?attr/colorPrimary"
            app:popupTheme="@style/AppTheme.PopupOverlay" />
```
在styles.xml中
```xml
 <style name="AppTheme.NoActionBar">
        <item name="windowActionBar">false</item>
        <item name="windowNoTitle">true</item>
    </style>
```
在styles.xml(V21)中
```xml
    <style name="AppTheme.NoActionBar">
        <item name="windowActionBar">false</item>
        <item name="windowNoTitle">true</item>
        <item name="android:windowDrawsSystemBarBackgrounds">true</item>
        <item name="android:statusBarColor">@android:color/transparent</item>
    </style>
```
[http://blog.csdn.net/lmj623565791/article/details/45303349](http://blog.csdn.net/lmj623565791/article/details/45303349 "参考")
###自定义menu菜单栏###
在menu文件夹中建立menu_main2.xml
在Activity中重写
```java
 @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_main2,menu);
        return true;
    }
```
设置子activity中的toolbar添加返回箭头
1. AndroidManifest.xml中给需要在toolbar上增加返回按钮的activity增加属性

    android:parentActivityName=”com.example.myfirstapp.MainActivity”
2.  `getActionBar().setDisplayHomeAsUpEnabled(true)`;

##AppBarLayout##


##CardView##
需要在gradle中引进`compile 'com.android.support:cardview-v7:21.0.2'`
```xml
    <android.support.v7.widget.CardView
        android:layout_width="150dp"
        android:layout_height="wrap_content"
        card_view:cardUseCompatPadding="true"
        card_view:cardCornerRadius="4dp"
        >
        <TextView
            android:id="@+id/id_num"
            android:layout_width="fill_parent"
            android:gravity="center"
            android:layout_height="wrap_content" />

    </android.support.v7.widget.CardView>
```
CardView中常用的属性有：

1. cardElevation:设置阴影的大小
2.  cardBackgroundColor:卡片布局的背景演示
3. cardCornerRadius：卡片布局的圆角的大小
4.  conentPadding：卡片布局和内容之间的距离
  
```xml
 		app:cardElevation="10dp"
        app:contentPadding="10dp"
```
##RecyclerView##
```xml
    <android.support.v7.widget.RecyclerView
        android:id="@+id/recyclerview"
        android:layout_width="match_parent"
        android:layout_height="match_parent">
    </android.support.v7.widget.RecyclerView>
```
内容设置
```java
//mLayoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);线性布局
//mLayoutManager = new GridLayoutManager(context,columNum);网格布局
//瀑布流
 recyclerview.setLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.VERTICAL));
        recyclerview.setAdapter(mAdapter = new HomeAdapter());
```
```java
 class HomeAdapter extends RecyclerView.Adapter<HomeAdapter.MyViewHolder> {

        @Override
        public MyViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            MyViewHolder holder = new MyViewHolder(LayoutInflater.from(
                    MainActivity.this).inflate(R.layout.item_layout, parent,
                    false));
            return holder;
        }

        @Override
        public void onBindViewHolder(MyViewHolder holder, int position) {
            holder.tv.setText(mDatas.get(position));
            Bitmap bitmap = BitmapFactory.decodeResource(getResources(),
                    R.drawable.a);
       holder.img.setImageBitmap(bitmap);
        }

        @Override
        public int getItemCount() {
            return mDatas.size();
        }

        class MyViewHolder extends RecyclerView.ViewHolder {

            TextView tv;
            ImageView img;

            public MyViewHolder(View view) {
                super(view);
                tv = (TextView) view.findViewById(R.id.id_num);
                img = (ImageView) view.findViewById(R.id.img);
            }
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
```