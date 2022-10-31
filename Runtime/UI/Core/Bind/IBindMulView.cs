namespace Framework
{
    /// <summary>
    /// 为了实现一个list vm绑定多个不同的view
    /// 可以用在聊天列表，同一个list用在两个不同的view上
    /// </summary>
    public interface IBindMulView
    {
        int Tag { get; set; }
    }
}