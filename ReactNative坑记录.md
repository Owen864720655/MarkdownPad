## React Native中的一些坑哦: ##
1. react native Text 上无法指定borderWidth 等一系列属性
```jsx
<View style={styles.list}><Text >{rowData}</Text></View>
```
2. listview不能滑动
在listview外层的View中添加 flex:1(其实是要固定高度就行，height:400也可以)
```jsx
  <View style={{paddingTop: 22, flex: 1}}>
     <ListView
         ataSource={this.state.dataSource}
          renderRow={(rowData) => <View style={styles.list}><Text >{rowData}</Text></View>}
     />
  </View>
```