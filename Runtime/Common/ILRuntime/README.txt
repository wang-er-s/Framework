1.Hotfix中不要写大量复杂的计算,特别是在Update之类的方法中.
2.Hotfix中调用泛型需要写重定向，参考ILRuntimeRedirectHelper中的各个注册
3.Hotfix对多线程Thread不兼容,使用会导致Unity崩溃闪退
4.Hotfix中重写Unity中的虚函数时,不能再调base.xxx(),否则会爆栈,也就是StackOverflow
5.Hotfix中不支持unsafe,intptr,interup
6.Unity调用Hotfix的方法,开销相对较大,尽量避免.
7.大部分情况下，Struct会比class慢，还会有频繁装箱的问题
8.不支持可空的值类型
10.使用for代替foreach，如果要使用Dic，可以增加一个list配合遍历
11.主项目中 创建实例化和GetType使用 ReflectionHelper 中的方法
12.尽量使用Action以及Func这两个系统内置万用委托类型
13.获取属性使用type.GetCustomAttributes(Typeof(Attr), false);
14.不要使用Get(Add)Component(Type)来操作组件
15.使用[][]代替 [,]
16.跨域继承不能多继承  如之前的Army跨域继承了Mono，如果再继承IDraggable，List<IDraggable>是不能识别的，所以需要热更里有一个类
继承Mono的同时实现IDraggable接口，Army继承DraggableMono，使用List<DraggableMono> 来接Army
17.不能使用ref/out  例如TryGetComponent
18.使用Release编译的dll运行，需要生成clr绑定并初始化
19.热更中不要定义枚举，一定要用则使用int来代替
20.Attribute的构造函数只能使用基本类型，不能使用params 数组
21.热更中字典如果使用继承自主项目的类做值，不能使用tryGetValue
22.继承mono的，不能在构造参数设置值或者给默认值，只能在awake中
23.跨域继承的子类无法调用当前重载以外的虚函数，跨域继承中不能在基类的构造函数中调用该类的虚函数

IL2CPP打包注意：
    会自动开启代码剔除
    使用CLR自动分析绑定可最大化避免裁剪
    适当使用link.xml来预留接口和类型
    尤其注意泛型方法和类型