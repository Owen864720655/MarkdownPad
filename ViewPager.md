# ViewPager #

----------
1. 需要一个视图的List  `List<View> views = new ArrayList<>();`
2. 需要一个ViewPager
```java
    private class MyPagerAdaper extends PagerAdapter {
        /**
         * Return the number of views available.
         */
        @Override
        public int getCount() {
            return views.size();
        }

        @Override
        public boolean isViewFromObject(View view, Object object) {
            return view == object;
        }

        @Override
        public void destroyItem(ViewGroup container, int position, Object object) {
            container.removeView(views.get(position));
        }

        @Override
        public Object instantiateItem(ViewGroup container, int position) {
            container.addView(views.get(position));
            return views.get(position);
        }
    }
```

关于View可以Layout
```java
  LayoutInflater inflater = getLayoutInflater();

        View view1 = inflater.inflate(R.layout.view1, null);
        views.add(view1);
```
也可以新建
```java
    ImageView imageView = new ImageView(this);
    // 异步加载图片
    Picasso.with(BannerActivity.this).load(bannerDatanList.get(i).getImageUrl()).resize(width, 550).into(imageView);
    mageViews.add(imageView);
```
监听事件：
```java
    private class MyPageChangeListener implements ViewPager.OnPageChangeListener {
        private int oldPosition = 0;

        @Override
        public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {

        }

        @Override
        public void onPageSelected(int position) {
            currentItem = position;
            BannerData bannerData = bannerDatanList.get(position);
            dots.get(oldPosition).setBackgroundResource(R.drawable.dot_normal);
            dots.get(position).setBackgroundResource(R.drawable.dot_focused);
            oldPosition = position;

        }

        @Override
        public void onPageScrollStateChanged(int state) {

        }
    }
```
