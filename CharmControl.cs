﻿#region 文档说明
/* *****************************************************************************************************
 * 文档作者：无闻
 * 创建日期：2013年2月18日
 * 文档用途：CharmControl类，保存控件基础属性，所有抽象控件均继承自它
 * -----------------------------------------------------------------------------------------------------
 * 修改记录：
 * 
 * -----------------------------------------------------------------------------------------------------
 * 参考文献：
 * C# 方法的 隐藏(new)、重写(override)、重载(overload)、多态 的区别 简介：http://xwj.ok.2008.blog.163.com/blog/static/6413918920107453051131/
 * *****************************************************************************************************/
#endregion

#region 命名空间引用
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using CharmCommonMethod;
#endregion

namespace CharmControlLibrary
{
    #region 枚举类型
    /// <summary>
    /// 控件状态：常态，悬浮态，按下态，失活态
    /// </summary>
    public enum ControlStatus : int
    {
        /// <summary>
        /// 常态
        /// </summary>
        Normal,
        /// <summary>
        /// 悬浮态：鼠标置于控件上方时
        /// </summary>
        Hover,
        /// <summary>
        /// 按下态：鼠标按下控件时
        /// </summary>
        Down,
        /// <summary>
        /// 失活态：控件被禁止时
        /// </summary>
        Unenabled
    }

    /// <summary>
    /// 控件类型
    /// </summary>
    public enum ControlType : int
    {
        /// <summary>
        /// 系统按钮控件
        /// </summary>
        CharmSysButton,
        /// <summary>
        /// 按钮控件
        /// </summary>
        CharmButton,
        /// <summary>
        /// 检查框控件
        /// </summary>
        CharmCheckBox,
        /// <summary>
        /// 链接标签控件
        /// </summary>
        CharmLinkLabel
    }
    #endregion

    /// <summary>
    /// CharmControl：Charm控件基类，保存Charm控件基础属性，所有Charm控件均继承自它
    /// </summary>
    public class CharmControl : IDropTarget, IDisposable
    {
        #region 字段
        // 系统按钮类型
        private SysButtonType mSysButtonType;
        // 按钮类型
        private ButtonType mButtonType;
        // 检查框类型
        private CheckBoxType mCheckBoxType;

        // 该控件的左上角相对于其容器的左上角的坐标
        private Point mLocation;
        // 控件左边缘与其容器的工作区左边缘之间的距离（以像素为单位）
        private int mLeft;
        // 控件上边缘与其容器的工作区上边缘之间的距离（以像素为单位）
        private int mTop;
        // 控件的高度和宽度
        private Size mSize;
        // 控件下边缘与其容器的工作区上边缘之间的距离（以像素为单位）
        private int mBottom;
        // 控件的工作区的矩形
        private Rectangle mClientRectangle;

        // 与此控件关联的文本
        private string mText;
        // 与此控件关联的文本位置
        private Point mTextPosition;
        // 控件显示的文字的字体
        private Font mFont;
        // 控件的前景色
        private Color mForeColor;
        // 控件的缓存前景色（用于在非激活态时保存原本的前景色）
        private Color mCacheForeColor;

        // 指示是否显示该控件及其所有子控件
        private bool mIsVisible;
        // 指示控件是否可以对用户交互作出响应
        private bool mIsEnabled;
        // 指示检查框是否被选中
        private bool mIsChecked;

        // 示控件是否可以接受用户拖放到它上面的数据
        private bool mIsAllowDrop;
        // 在控件中显示的背景图像
        private Image mBackgroundImage;
        // 控件的工作区的高度和宽度
        private Size mClientSize;
        // 控件的应用程序的公司名称或创建者
        private string mCompanyName;
        // 控件关联的快捷菜单
        private CharmMenu mContextMenu;
        // 包含在控件内的控件的集合
        private List<CharmControl> mControls;
        // 控件的状态
        private ControlStatus mControlStatus;
        // 控件的类型
        private ControlType mControlType;
        // 当鼠标指针位于控件上时显示的光标
        private Cursor mCursor;
        // 指示控件是否有输入焦点
        private bool mIsFocused;
        // 控件绑定到的窗口句柄
        private IntPtr mHandle;
        // 控件的高度
        private int mHeight;
        // 控件的名称
        private string mName;
        // 控件的父容器
        private Control mParent;
        // 包含控件的程序集的产品名称
        private string mProductName;
        // 包含控件的程序集的版本
        private string mProductVersion;
        // 在控件的容器的控件的 Tab 键顺序
        private int mTabIndex;
        // 指示用户能否使用 Tab 键将焦点放到该控件上
        private bool mIsTabStop;
        // 控件的宽度
        private int mWidth;
        // 控件的工具提示文本
        private string mToolTipText;
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置 CharmControlLibrary.CharmSysButton 的系统按钮类型
        /// </summary>
        public virtual SysButtonType SysButtonType
        {
            get { return this.mSysButtonType; }
            set { this.mSysButtonType = value; }
        }

        /// <summary>
        /// 获取或设置 CharmControlLibrary.CharmButton 的按钮类型
        /// </summary>
        public virtual ButtonType ButtonType
        {
            get { return this.mButtonType; }
            set { this.mButtonType = value; }
        }

        /// <summary>
        /// 获取或设置 CharmControlLibrary.CharmCheckBox 的检查框类型
        /// </summary>
        public virtual CheckBoxType CheckBoxType
        {
            get { return mCheckBoxType; }
            set { mCheckBoxType = value; }
        }

        /// <summary>
        /// 获取或设置该控件的左上角相对于其容器的左上角的坐标
        /// </summary>
        public Point Location
        {
            get { return this.mLocation; }
            set
            {
                this.mLocation = value;
                this.mLeft = mLocation.X;
                this.mTop = mLocation.Y;
                this.mClientRectangle.Location = mLocation;
            }
        }

        /// <summary>
        /// 获取或设置控件左边缘与其容器的工作区左边缘之间的距离（以像素为单位）
        /// </summary>
        public int Left
        {
            get { return this.mLeft; }
            set
            {
                this.mLeft = value;
                this.mLocation.X = mLeft;
                this.mClientRectangle.X = mLeft;
            }
        }

        /// <summary>
        /// 获取或设置控件上边缘与其容器的工作区上边缘之间的距离（以像素为单位）
        /// </summary>
        public int Top
        {
            get { return this.mTop; }
            set
            {
                this.mTop = value;
                this.mLocation.Y = mTop;
                this.mClientRectangle.Y = mTop;
                this.mBottom = mClientRectangle.Bottom;
            }
        }

        /// <summary>
        /// 获取或设置控件的高度和宽度
        /// </summary>
        public Size Size
        {
            get { return this.mSize; }
            set
            {
                this.mSize = value;
                this.mClientRectangle.Size = mSize;
                this.mClientSize = mSize;
                this.mHeight = mSize.Height;
                this.mWidth = mSize.Width;
                this.mBottom = mClientRectangle.Bottom;
            }
        }

        /// <summary>
        /// 获取控件下边缘与其容器的工作区上边缘之间的距离（以像素为单位）
        /// </summary>
        public int Bottom
        {
            get { return this.mBottom; }
        }

        /// <summary>
        /// 获取表示控件的工作区的矩形
        /// </summary>
        public Rectangle ClientRectangle
        {
            get { return this.mClientRectangle; }
        }

        /// <summary>
        /// 获取或设置与此控件关联的文本
        /// </summary>
        public virtual string Text
        {
            get { return this.mText; }
            set { this.mText = value; }
        }

        /// <summary>
        /// 获取或设置与此控件关联的文本位置
        /// </summary>
        public Point TextPosition
        {
            get { return this.mTextPosition; }
            set { this.mTextPosition = value; }
        }

        /// <summary>
        /// 获取或设置控件显示的文字的字体
        /// </summary>
        public virtual Font Font
        {
            get { return this.mFont; }
            set { this.mFont = value; }
        }

        /// <summary>
        /// 获取或设置控件的前景色
        /// </summary>
        public virtual Color ForeColor
        {
            get { return this.mForeColor; }
            set { this.mForeColor = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示该控件及其所有子控件
        /// </summary>
        public bool Visible
        {
            get { return this.mIsVisible; }
            set { this.mIsVisible = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示控件是否可以对用户交互作出响应
        /// </summary>
        public virtual bool Enabled
        {
            get { return this.mIsEnabled; }
            set
            {
                this.mIsEnabled = value;
                // 根据控件激活性设置控件状态
                if (this.mIsEnabled)
                {
                    this.mForeColor = this.mCacheForeColor;
                }
                else
                {
                    this.mCacheForeColor = this.mForeColor;
                    this.mForeColor = Color.DarkGray;
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示检查框是否被选中
        /// </summary>
        public virtual bool Checked
        {
            get { return this.mIsChecked; }
            set
            {
                this.mIsChecked = value;
                // 修改状态是为了让主窗体能重绘该控件
                this.ControlStatus = ControlStatus.Normal;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示控件是否可以接受用户拖放到它上面的数据
        /// </summary>
        public virtual bool AllowDrop
        {
            get { return this.mIsAllowDrop; }
            set { this.mIsAllowDrop = value; }
        }

        /// <summary>
        /// 获取或设置在控件中显示的背景图像
        /// </summary>
        public virtual Image BackgroundImage
        {
            get { return this.mBackgroundImage; }
            set { this.mBackgroundImage = value; }
        }

        /// <summary>
        /// 获取或设置控件的工作区的高度和宽度
        /// </summary>
        public Size ClientSize
        {
            get { return this.mClientSize; }
            set { this.mClientSize = value; }
        }

        /// <summary>
        /// 获取包含控件的应用程序的公司名称或创建者
        /// </summary>
        public string CompanyName
        {
            get { return this.mCompanyName; }
        }

        /// <summary>
        /// 获取或设置与控件关联的快捷菜单
        /// </summary>
        public virtual CharmMenu ContextMenu
        {
            get { return this.mContextMenu; }
            set { this.mContextMenu = value; }
        }

        /// <summary>
        /// 获取包含在控件内的控件的集合
        /// </summary>
        public List<CharmControl> Controls
        {
            get { return this.mControls; }
        }

        /// <summary>
        /// 获取或设置控件的状态
        /// </summary>
        public virtual ControlStatus ControlStatus
        {
            get { return this.mControlStatus; }
            set { this.mControlStatus = value; }
        }

        /// <summary>
        /// 获取或设置控件的类型
        /// </summary>
        public ControlType ControlType
        {
            get { return this.mControlType; }
            set { this.mControlType = value; }
        }

        /// <summary>
        /// 获取或设置当鼠标指针位于控件上时显示的光标
        /// </summary>
        public virtual Cursor Cursor
        {
            get { return this.mCursor; }
            set { this.mCursor = value; }
        }

        /// <summary>
        /// 获取一个值，该值指示控件是否有输入焦点
        /// </summary>
        public virtual bool Focused
        {
            get { return this.mIsFocused; }
        }

        /// <summary>
        /// 获取控件绑定到的窗口句柄
        /// </summary>
        public IntPtr Handle
        {
            get { return this.mHandle; }
        }

        /// <summary>
        /// 获取或设置控件的高度
        /// </summary>
        public int Height
        {
            get { return this.mHeight; }
            set
            {
                this.mHeight = value;
                this.mClientRectangle.Height = mHeight;
                this.mClientSize.Height = mHeight;
                this.mSize.Height = mHeight;
                this.mBottom = mClientRectangle.Bottom;
            }
        }

        /// <summary>
        /// 获取或设置控件的名称
        /// </summary>
        public string Name
        {
            get { return this.mName; }
            set { this.mName = value; }
        }

        /// <summary>
        /// 获取或设置控件的父容器
        /// </summary>
        public Control Parent
        {
            get { return this.mParent; }
            set { this.mParent = value; }
        }

        /// <summary>
        /// 获取包含控件的程序集的产品名称
        /// </summary>
        public string ProductName
        {
            get { return this.mProductName; }
        }

        /// <summary>
        /// 获取包含控件的程序集的版本
        /// </summary>
        public string ProductVersion
        {
            get { return this.mProductVersion; }
        }

        /// <summary>
        /// 获取或设置在控件的容器的控件的 Tab 键顺序
        /// </summary>
        public int TabIndex
        {
            get { return this.mTabIndex; }
            set { this.mTabIndex = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示用户能否使用 Tab 键将焦点放到该控件上
        /// </summary>
        public bool TabStop
        {
            get { return this.mIsTabStop; }
            set { this.mIsTabStop = value; }
        }

        /// <summary>
        /// 获取或设置控件的宽度
        /// </summary>
        public int Width
        {
            get { return this.mWidth; }
            set 
            {
                this.mWidth = value;
                this.mClientRectangle.Width = mWidth;
                this.mClientSize.Width = mWidth;
                this.mSize.Width = mWidth;
            }
        }

        /// <summary>
        /// 获取或设置控件的工具提示文本
        /// </summary>
        public string ToolTipText
        {
            get { return this.mToolTipText; }
            set { this.mToolTipText = value; }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 当 BackgroundImage 属性的值更改时发生
        /// </summary>
        public event EventHandler BackgroundImageChanged;
        /// <summary>
        /// 引发 BackgroundImageChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnBackgroundImageChanged(
            EventArgs e)
        {
            if (this.BackgroundImageChanged != null)
                this.BackgroundImageChanged(this, e);
        }

        /// <summary>
        /// 在单击控件时发生
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// 引发 Click 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnClick(
            EventArgs e)
        {
            if (this.Click != null)
                this.Click(this, e);
        }

        /// <summary>
        /// 当 ClientSize 属性的值更改时发生
        /// </summary>
        public event EventHandler ClientSizeChanged;
        /// <summary>
        /// 引发 ClientSizeChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnClientSizeChanged(
            EventArgs e)
        {
            if (this.ClientSizeChanged != null)
                this.ClientSizeChanged(this, e);
        }

        /// <summary>
        /// 当 ContextMenu 属性的值更改时发生
        /// </summary>
        public event EventHandler ContextMenuChanged;
        /// <summary>
        /// 引发 ContextMenuChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnContextMenuChanged(
            EventArgs e)
        {
            if (this.ContextMenuChanged != null)
                this.ContextMenuChanged(this, e);
        }

        /// <summary>
        /// 在将新控件添加到 List.CharmControl 时发生
        /// </summary>
        public event ControlEventHandler ControlAdded;
        /// <summary>
        /// 引发 ControlAdded 事件
        /// </summary>
        /// <param name="e">包含事件数据的 ControlEventArgs</param>
        protected virtual void OnControlAdded(
            ControlEventArgs e)
        {
            if (this.ControlAdded != null)
                this.ControlAdded(this, e);
        }

        /// <summary>
        /// 在从 List.CharmControl 移除控件时发生
        /// </summary>
        public event ControlEventHandler ControlRemoved;
        /// <summary>
        /// 引发 ControlRemoved 事件
        /// </summary>
        /// <param name="e">包含事件数据的 ControlEventArgs</param>
        protected virtual void OnControlRemoved(
            ControlEventArgs e)
        {
            if (this.ControlRemoved != null)
                this.ControlRemoved(this, e);
        }

        /// <summary>
        /// 当 Cursor 属性的值更改时发生
        /// </summary>
        public event EventHandler CursorChanged;
        /// <summary>
        /// 引发 CursorChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnCursorChanged(
            EventArgs e)
        {
            if (this.CursorChanged != null)
                this.CursorChanged(this, e);
        }

        /// <summary>
        /// 当通过调用 Dispose 方法释放组件时发生
        /// </summary>
        public event EventHandler Disposed;
        /// <summary>
        /// 释放由 Component 使用的所有资源
        /// </summary>
        public void Dispose()
        {
            // * 释放系统资源 *
            this.mBackgroundImage = null;
            this.mFont = null;
            // 触发事件
            if (this.Disposed != null)
                this.Disposed(this, new EventArgs());
        }

        /// <summary>
        /// 在双击控件时发生
        /// </summary>
        public event EventHandler DoubleClick;
        /// <summary>
        /// 引发 DoubleClick 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnDoubleClick(
            EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(this, e);
        }

        /// <summary>
        /// 在完成拖放操作时发生
        /// </summary>
        public event DragEventHandler DragDrop;
        /// <summary>
        /// 引发 DragDrop 事件
        /// </summary>
        /// <param name="e">包含事件数据的 DragEventArgs</param>
        public void OnDragDrop(
            DragEventArgs e)
        {
            if (this.DragDrop != null)
                this.DragDrop(this, e);
        }

        /// <summary>
        /// 在将对象拖入控件的边界时发生
        /// </summary>
        public event DragEventHandler DragEnter;
        /// <summary>
        /// 引发 DragEnter 事件
        /// </summary>
        /// <param name="e">包含事件数据的 DragEventArgs</param>
        public void OnDragEnter(
            DragEventArgs e)
        {
            if (this.DragEnter != null)
                this.DragEnter(this, e);
        }

        /// <summary>
        /// 在将对象拖出控件的边界时发生
        /// </summary>
        public event EventHandler DragLeave;
        /// <summary>
        /// 引发 DragLeave 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        public void OnDragLeave(
            EventArgs e)
        {
            if (this.DragLeave != null)
                this.DragLeave(this, e);
        }

        /// <summary>
        /// 在将对象拖到控件的边界上发生
        /// </summary>
        public event DragEventHandler DragOver;
        /// <summary>
        /// 引发 DragOver 事件
        /// </summary>
        /// <param name="e">包含事件数据的 DragEventArgs</param>
        public void OnDragOver(
            DragEventArgs e)
        {
            if (this.DragOver != null)
                this.DragOver(this, e);
        }

        /// <summary>
        /// 在 Enabled 属性值更改后发生
        /// </summary>
        public event EventHandler EnabledChanged;
        /// <summary>
        /// 引发 EnabledChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        public void OnEnabledChanged(
            EventArgs e)
        {
            if (this.EnabledChanged != null)
                this.EnabledChanged(this, e);
        }

        /// <summary>
        /// 在控件接收焦点时发生
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// 引发 GotFocus 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        public void OnGotFocus(
            EventArgs e)
        {
            if (this.GotFocus != null)
                this.GotFocus(this, e);
        }

        /// <summary>
        /// 在控件的显示需要重绘时发生
        /// </summary>
        public event InvalidateEventHandler Invalidated;
        /// <summary>
        /// 引发 Invalidated 事件
        /// </summary>
        /// <param name="e">包含事件数据的 InvalidateEventArgs</param>
        protected virtual void OnInvalidated(
            InvalidateEventArgs e)
        {
            if (this.Invalidated != null)
                this.Invalidated(this, e);
        }

        /// <summary>
        /// 在控件有焦点的情况下按下键时发生
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// 引发 KeyDown 事件
        /// </summary>
        /// <param name="e">包含事件数据的 KeyEventArgs</param>
        protected virtual void OnKeyDown(
            KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(this, e);
        }

        /// <summary>
        /// 在控件有焦点的情况下按下键时发生
        /// </summary>
        public event KeyPressEventHandler KeyPress;
        /// <summary>
        /// 引发 KeyPress 事件
        /// </summary>
        /// <param name="e">包含事件数据的 KeyPressEventArgs</param>
        protected virtual void OnKeyPress(
            KeyPressEventArgs e)
        {
            if (this.KeyPress != null)
                this.KeyPress(this, e);
        }

        /// <summary>
        /// 在控件有焦点的情况下释放键时发生
        /// </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// 引发 KeyUp 事件
        /// </summary>
        /// <param name="e">包含事件数据的 KeyEventArgs</param>
        protected virtual void OnKeyUp(
            KeyEventArgs e)
        {
            if (this.KeyUp != null)
                this.KeyUp(this, e);
        }

        /// <summary>
        /// 在输入焦点离开控件时发生
        /// </summary>
        public event EventHandler Leave;
        /// <summary>
        /// 引发 Leave 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnLeave(
            EventArgs e)
        {
            if (this.Leave != null)
                this.Leave(this, e);
        }

        /// <summary>
        /// 在 Location 属性值更改后发生
        /// </summary>
        public event EventHandler LocationChanged;
        /// <summary>
        /// 引发 LocationChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnLocationChanged(
            EventArgs e)
        {
            if (this.LocationChanged != null)
                this.LocationChanged(this, e);
        }

        /// <summary>
        /// 当控件失去焦点时发生
        /// </summary>
        public event EventHandler LostFocus;
        /// <summary>
        /// 引发 LostFocus 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnLostFocus(
            EventArgs e)
        {
            if (this.LostFocus != null)
                this.LostFocus(this, e);
        }

        /// <summary>
        /// 在鼠标单击该控件时发生
        /// </summary>
        public event MouseEventHandler MouseClick;
        /// <summary>
        /// 引发 MouseClick 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseClick(
            MouseEventArgs e)
        {
            if (this.MouseClick != null)
                this.MouseClick(this, e);
        }

        /// <summary>
        /// 当用鼠标双击控件时发生
        /// </summary>
        public event MouseEventHandler MouseDoubleClick;
        /// <summary>
        /// 引发 MouseDoubleClick 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseDoubleClick(
            MouseEventArgs e)
        {
            if (this.MouseDoubleClick != null)
                this.MouseDoubleClick(this, e);
        }

        /// <summary>
        /// 当鼠标指针位于控件上并按下鼠标键时发生
        /// </summary>
        public event MouseEventHandler MouseDown;
        /// <summary>
        /// 引发 MouseDown 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseDown(
            MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(this, e);
        }

        /// <summary>
        /// 在鼠标指针进入控件时发生
        /// </summary>
        public event EventHandler MouseEnter;
        /// <summary>
        /// 引发 MouseEnter 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnMouseEnter(
            EventArgs e)
        {
            if (this.MouseEnter != null)
                this.MouseEnter(this, e);
        }

        /// <summary>
        /// 在鼠标指针停放在控件上时发生
        /// </summary>
        public event EventHandler MouseHover;
        /// <summary>
        /// 引发 MouseHover 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnMouseHover(
            EventArgs e)
        {
            if (this.MouseHover != null)
                this.MouseHover(this, e);
        }

        /// <summary>
        /// 在鼠标指针离开控件时发生
        /// </summary>
        public event EventHandler MouseLeave;
        /// <summary>
        /// 引发 MouseLeave 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnMouseLeave(
            EventArgs e)
        {
            if (this.MouseLeave != null)
                this.MouseLeave(this, e);
        }

        /// <summary>
        /// 在鼠标指针移到控件上时发生
        /// </summary>
        public event MouseEventHandler MouseMove;
        /// <summary>
        /// 引发 MouseMove 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseMove(
            MouseEventArgs e)
        {
            if (this.MouseMove != null)
                this.MouseMove(this, e);
        }

        /// <summary>
        /// 在鼠标指针在控件上并释放鼠标键时发生
        /// </summary>
        public event MouseEventHandler MouseUp;
        /// <summary>
        /// 引发 MouseUp 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseUp(
            MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(this, e);
        }

        /// <summary>
        /// 在移动鼠标滚轮并且控件有焦点时发生
        /// </summary>
        public event MouseEventHandler MouseWheel;
        /// <summary>
        /// 引发 MouseWheel 事件
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        protected virtual void OnMouseWheel(
            MouseEventArgs e)
        {
            if (this.MouseWheel != null)
                this.MouseWheel(this, e);
        }

        /// <summary>
        /// 在移动控件时发生
        /// </summary>
        public event EventHandler Move;
        /// <summary>
        /// 引发 Move 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnMove(
            EventArgs e)
        {
            if (this.Move != null)
                this.Move(this, e);
        }

        /// <summary>
        /// 在重绘控件时发生
        /// </summary>
        public event PaintEventHandler Paint;
        /// <summary>
        /// 引发 Paint 事件
        /// </summary>
        /// <param name="e">包含事件数据的 PaintEventArgs</param>
        protected virtual void OnPaint(
            PaintEventArgs e)
        {
            if (this.Paint != null)
                this.Paint(this, e);
        }

        /// <summary>
        /// 在焦点位于此控件上的情况下，当有按键动作时发生（在 KeyDown 事件之前发生）
        /// </summary>
        public event PreviewKeyDownEventHandler PreviewKeyDown;
        /// <summary>
        /// 引发 PreviewKeyDown 事件
        /// </summary>
        /// <param name="e">包含事件数据的 PreviewKeyDownEventArgs</param>
        protected virtual void OnPreviewKeyDown(
            PreviewKeyDownEventArgs e)
        {
            if (this.PreviewKeyDown != null)
                this.PreviewKeyDown(this, e);
        }

        /// <summary>
        /// 在调整控件大小时发生
        /// </summary>
        public event EventHandler Resize;
        /// <summary>
        /// 引发 Resize 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnResize(
            EventArgs e)
        {
            if (this.Resize != null)
                this.Resize(this, e);
        }

        /// <summary>
        /// 在 Size 属性值更改时发生
        /// </summary>
        public event EventHandler SizeChanged;
        /// <summary>
        /// 引发 SizeChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnSizeChanged(
            EventArgs e)
        {
            if (this.SizeChanged != null)
                this.SizeChanged(this, e);
        }

        /// <summary>
        /// 在 Text 属性值更改时发生
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// 引发 TextChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnTextChanged(
            EventArgs e)
        {
            if (this.TextChanged != null)
                this.TextChanged(this, e);
        }

        /// <summary>
        /// 在 Visible 属性值更改时发生
        /// </summary>
        public event EventHandler VisibleChanged;
        /// <summary>
        /// 引发 VisibleChanged 事件
        /// </summary>
        /// <param name="e">包含事件数据的 EventArgs</param>
        protected virtual void OnVisibleChanged(
            EventArgs e)
        {
            if (this.VisibleChanged != null)
                this.VisibleChanged(this, e);
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 用默认设置初始化 Control 类的新实例
        /// </summary>
        public CharmControl()
            : this(string.Empty)
        {

        }

        /// <summary>
        /// 用特定的文本初始化 Control 类的新实例
        /// </summary>
        /// <param name="text">控件显示的文本</param>
        public CharmControl(
            string text)
            : this(text, 0, 0, 100, 20)
        {

        }

        /// <summary>
        /// 用特定的文本、大小和位置初始化 Control 类的新实例
        /// </summary>
        /// <param name="text">控件显示的文本</param>
        /// <param name="left">控件距其容器左边缘的 X 位置（以像素为单位）。 该值被分配给 Left 属性</param>
        /// <param name="top">控件距其容器上边缘的 Y 位置（以像素为单位）。 该值被分配给 Top 属性</param>
        /// <param name="width">控件的宽度（以像素为单位）。该值被分配给 Width 属性</param>
        /// <param name="height">控件的高度（以像素为单位）。该值被分配给 Height 属性</param>
        public CharmControl(
            string text,
            int left,
            int top,
            int width,
            int height)
        {
            // * 版权声明 *
            this.mProductName = "CharmControlLibrary";
            this.mCompanyName = "CSBox工作室";
            this.mProductVersion = "2.0.*";

            // * 初始化属性 *
            this.mText = text;
            this.mLeft = left;
            this.mTop = top;
            this.mWidth = width;
            this.mHeight = height;

            this.mBackgroundImage = new Bitmap(width, height);
            this.mClientRectangle = new Rectangle(left, top, width, height);
            this.mSize = mClientRectangle.Size;
            this.mLocation = mClientRectangle.Location;
            this.mClientSize = mClientRectangle.Size;
            this.mControls = new List<CharmControl>();
            this.mControlStatus = ControlStatus.Normal;
            this.mCursor = Cursors.Arrow;
            this.mFont = new Font("微软雅黑", 9);
            this.mForeColor = Color.Black;
            Random rand = new Random();
            this.mHandle = (IntPtr)rand.Next(100000, 999999);
            this.mName = string.Empty;
            this.mTabIndex = 0;
            this.mIsTabStop = true;
            this.mIsAllowDrop = false;
            this.mIsEnabled = true;
            this.mIsFocused = false;
            this.mIsVisible = true;

            // * 释放系统资源 *
            rand = null;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 检索一个值，该值指示指定控件是否为一个控件的子控件
        /// </summary>
        /// <param name="cctl">要计算的 CharmControl</param>
        /// <returns>如果指定控件是控件的子控件，则为 true；否则为 false</returns>
        public bool Contains(
            CharmControl cctl)
        {
            return true;
        }

        /// <summary>
        /// 释放由 CharmControl 和它的子控件占用的非托管资源，另外还可以释放托管资源
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源</param>
        public void Dispose(
            bool disposing)
        {

        }

        /// <summary>
        /// 开始拖放操作
        /// </summary>
        /// <param name="data">要拖动的数据</param>
        /// <param name="allowedEffects">DragDropEffects 值之一</param>
        /// <returns>DragDropEffects 枚举的值，它表示在拖放操作期间执行的最终效果</returns>
        public DragDropEffects DoDragDrop(
            object data,
            DragDropEffects allowedEffects)
        {
            return DragDropEffects.All;
        }

        /// <summary>
        /// 支持呈现到指定的位图
        /// </summary>
        /// <param name="bitmap">要绘制到的位图</param>
        /// <param name="targetBounds">呈现控件时的边界</param>
        public void DrawToBitmap(
            Bitmap bitmap,
            Rectangle targetBounds)
        {

        }

        /// <summary>
        /// 为控件设置输入焦点
        /// </summary>
        /// <returns>如果输入焦点请求成功，则为 true；否则为 false</returns>
        public bool Focus()
        {
            return true;
        }

        /// <summary>
        /// 检索包含指定句柄的控件
        /// </summary>
        /// <param name="handle">要搜索的窗口句柄 ( HWND)</param>
        /// <returns>CharmControl，它表示与指定句柄关联的控件；如果找不到带有指定句柄的控件，就返回 null</returns>
        public static CharmControl FromChildHandle(
            IntPtr handle)
        {
            return new CharmControl();
        }

        /// <summary>
        /// 返回当前与指定句柄关联的控件
        /// </summary>
        /// <param name="handle">要搜索的窗口句柄 ( HWND)</param>
        /// <returns>一个 CharmControl，它表示与指定句柄关联的控件；如果找不到带有指定句柄的控件，就返回 null</returns>
        public static CharmControl FromHandle(
            IntPtr handle)
        {
            return new CharmControl();
        }

        /// <summary>
        /// 按照子控件的 Tab 键顺序向前或向后检索下一个控件
        /// </summary>
        /// <param name="cctl">从其上开始搜索的 CharmControl</param>
        /// <param name="forward">如果是 true，则按 Tab 键顺序向前搜索；如果是 false 则向后搜索</param>
        /// <returns>Tab 键顺序指定的下一个 CharmControl</returns>
        public CharmControl GetNextControl(
            CharmControl cctl,
            bool forward)
        {
            return new CharmControl();
        }

        /// <summary>
        /// 对用户隐藏控件
        /// </summary>
        public void Hide()
        {

        }

        /// <summary>
        /// 使控件的整个图面无效并导致重绘控件
        /// </summary>
        public void Invalidate()
        {

        }

        /// <summary>
        /// 使控件的指定区域无效（将其添加到控件的更新区域，下次绘制操作时将重新绘制更新区域），并向控件发送绘制消息
        /// </summary>
        /// <param name="rc">一个 Rectangle，表示要使之无效的区域</param>
        public void Invalidate(
            Rectangle rc)
        {

        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 重绘事件处理
        /// </summary>
        /// <param name="g">Graphics 绘制对象</param>
        /// <param name="controlList">包含在控件内的控件的集合</param>
        public static void PaintEvent(
            Graphics g,
            List<CharmControl> controlList)
        {
            // 绘制过程需要用到的变量
            Image imgControl = null;              // 控件图像资源
            Point ptControl = new Point(0, 0);  // 控件绘制坐标
            Size siControl = new Size(0, 0);      // 控件绘制大小

            // 轮询控件集合
            foreach (CharmControl control in controlList)
            {
                // 判断控件是否可见
                if (!control.Visible)
                    continue;
                // 判断控件类型并发生行为
                switch (control.ControlType)
                {
                    case ControlType.CharmSysButton:
                    case ControlType.CharmButton:
                        // 检查用户是否完成对系统按钮或按钮的初始化
                        if (control.SysButtonType == SysButtonType.Undefined &&
                            control.ButtonType == ButtonType.Undefined)
                        {
                            throw new ArgumentException(
                                control.GetType() + "：未指定系统按钮或按钮类型.\n" +
                                "Name: " + control.Name);
                        }
                        imgControl = control.BackgroundImage;
                        ptControl = control.Location;
                        siControl = control.Size;
                        break;
                    case ControlType.CharmCheckBox:
                        imgControl = control.BackgroundImage;
                        ptControl = control.Location;
                        break;
                    case ControlType.CharmLinkLabel:
                        g.DrawString(control.Text, control.Font, new SolidBrush(control.ForeColor), control.Location);
                        break;
                }

                // 根据控件图像是否被赋值判断是否需要绘制控件
                if (imgControl != null)
                {
                    // 判断控件类型并绘制图像
                    switch (control.ControlType)
                    {
                        case ControlType.CharmCheckBox:
                            g.DrawImage(imgControl, new Rectangle(ptControl.X + 4, ptControl.Y + 4, 17, 17));
                            break;
                        default:
                            g.DrawImage(imgControl, new Rectangle(ptControl, siControl));
                            break;
                    }

                    // 判断控件类型并绘制文本
                    switch (control.ControlType)
                    {
                        case ControlType.CharmButton:
                        case ControlType.CharmCheckBox:
                            g.DrawString(control.Text, control.Font,
                                new SolidBrush(control.ForeColor),
                                new Point(control.Location.X + control.TextPosition.X, control.Location.Y + control.TextPosition.Y));
                            break;
                    }
                    imgControl = null;
                }
            }
        }

        /// <summary>
        /// 鼠标单击事件处理
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        /// <param name="controlList">包含在控件内的控件的集合</param>
        public static void MouseClickEvent(
            MouseEventArgs e,
            List<CharmControl> controlList)
        {
            // 轮询控件集合
            foreach (CharmControl control in controlList)
            {
                // 判断控件是否可见且非禁止态且在控件工作区内
                if (control.Visible &&
                    (int)control.ControlStatus < 3 &&
                    control.ClientRectangle.Contains(e.Location))
                {
                    switch (e.Button)   // 判断鼠标按钮
                    {
                        case MouseButtons.Left:
                            control.OnMouseClick(e);    // 触发控件事件
                            break;
                        case MouseButtons.Right:
                            break;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 鼠标按下事件处理
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        /// <param name="controlList">包含在控件内的控件的集合</param>
        /// <param name="parent">控件的父容器</param>
        /// <returns>返回：true=捕捉到事件; false=没有捕捉到事件</returns>
        public static bool MouseDownEvent(
            MouseEventArgs e,
            List<CharmControl> controlList,
            Control parent)
        {
            // 过程需要用到的变量
            List<Rectangle> rectRedrawList = new List<Rectangle>(); // 重绘区域集合

            // 轮询控件集合
            foreach (CharmControl control in controlList)
            {
                // 判断控件是否可见
                if (!control.Visible)
                    continue;
                // 判断控件类型并发生行为
                switch (control.ControlType)
                {
                    case ControlType.CharmSysButton:
                        if (control.ClientRectangle.Contains(e.Location))     // 判断是否在控件工作区内
                            switch (e.Button)   // 判断鼠标按钮
                            {
                                case MouseButtons.Left:
                                    if (control.ControlStatus != ControlStatus.Down)    // 判断是否已经是按下态
                                    {
                                        control.ControlStatus = ControlStatus.Down;      // 设置控件状态
                                        control.TextPosition = new Point(control.TextPosition.X + 1, control.TextPosition.Y + 1);
                                        rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                                        control.OnMouseDown(e);                                // 触发控件事件
                                    }
                                    break;
                                case MouseButtons.Right:
                                    break;
                            }
                        break;
                    case ControlType.CharmButton:
                        if (control.ControlStatus == ControlStatus.Unenabled)   // 判断按钮是否为禁止态
                            break;
                        else
                            goto case ControlType.CharmSysButton;   // 非禁止态处理过程与系统按钮一致
                    case ControlType.CharmCheckBox:     // 此处无实际作用，只为成功触发鼠标单击事件，否则会自动转为移动窗体事件
                    case ControlType.CharmLinkLabel:
                        if (control.ClientRectangle.Contains(e.Location))     // 判断是否在控件工作区内
                        {
                            switch (e.Button)   // 判断鼠标按钮
                            {
                                case MouseButtons.Left:
                                    rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                                    control.OnMouseDown(e);                                // 触发控件事件
                                    break;
                                case MouseButtons.Right:
                                    break;
                            }
                        }
                        break;
                }
            }

            // 判断是否存在需要重绘的控件
            if (rectRedrawList.Count > 0)
            {
                Rectangle rectRedraw = rectRedrawList[0];
                for (int i = 1; i < rectRedrawList.Count; i++)
                    rectRedraw = Rectangle.Union(rectRedraw, rectRedrawList[i]);
                parent.Invalidate(rectRedraw);
                return true;
            }
            // 没有捕捉到事件
            return false;
        }

        /// <summary>
        /// 鼠标移动事件处理
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        /// <param name="controlList">包含在控件内的控件的集合</param>
        /// <param name="parent">控件的父容器</param>
        /// <param name="toolTip">工具提示文本控件</param>
        public static void MouseMoveEvent(
            MouseEventArgs e,
            List<CharmControl> controlList,
            Control parent,
            ToolTip toolTip)
        {
            // 过程需要用到的变量
            List<Rectangle> rectRedrawList = new List<Rectangle>(); // 重绘区域集合

            // 轮询控件集合
            foreach (CharmControl control in controlList)
            {
                // 判断控件是否可见
                if (!control.Visible)
                    continue;
                // 判断控件类型并发生行为
                switch (control.ControlType)
                {
                    case ControlType.CharmSysButton:
                        if (control.ClientRectangle.Contains(e.Location))     // 判断是否在控件工作区内
                        {
                            // 判断是否已经是悬浮态或按下态
                            if (control.ControlStatus != ControlStatus.Hover &&
                                control.ControlStatus != ControlStatus.Down)
                            {
                                control.ControlStatus = ControlStatus.Hover;      // 设置控件状态
                                rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                                toolTip.Show(control.ToolTipText, parent,           // 设置工具提示文本
                                    new Point(control.Left, control.Bottom), 1000);
                                control.OnMouseMove(e);                                // 触发控件事件
                            }
                        }
                        else if (control.ControlStatus != ControlStatus.Normal)    // 判断是否已经是常态
                        {
                            if (control.ControlStatus == ControlStatus.Down)
                                control.TextPosition = new Point(control.TextPosition.X - 1, control.TextPosition.Y - 1);
                            control.ControlStatus = ControlStatus.Normal;      // 设置控件状态
                            rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                        }
                        break;
                    case ControlType.CharmButton:
                        if (control.ControlStatus == ControlStatus.Unenabled)   // 判断按钮是否为禁止态
                            break;
                        else
                            goto case ControlType.CharmSysButton;   // 非禁止态处理过程与系统按钮一致
                    case ControlType.CharmCheckBox:
                    case ControlType.CharmLinkLabel:
                        if (control.ClientRectangle.Contains(e.Location))     // 判断是否在控件工作区内
                        {
                            if (control.ControlStatus != ControlStatus.Hover)  // 判断是否已经是悬浮态
                            {
                                control.ControlStatus = ControlStatus.Hover;      // 设置控件状态
                                rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                                control.OnMouseMove(e);                                // 触发控件事件
                            }
                        }
                        else if (control.ControlStatus != ControlStatus.Normal)    // 判断是否已经是常态
                        {
                            control.ControlStatus = ControlStatus.Normal;      // 设置控件状态
                            rectRedrawList.Add(control.ClientRectangle);      // 将控件工作区的矩形添加至重绘集合
                        }
                        break;
                }
            }

            // 判断是否存在需要重绘的控件
            if (rectRedrawList.Count > 0)
            {
                Rectangle rectRedraw = rectRedrawList[0];
                for (int i = 1; i < rectRedrawList.Count; i++)
                    rectRedraw = Rectangle.Union(rectRedraw, rectRedrawList[i]);
                parent.Invalidate(rectRedraw);
            }
        }

        /// <summary>
        /// 鼠标弹起事件处理
        /// </summary>
        /// <param name="e">包含事件数据的 MouseEventArgs</param>
        /// <param name="controlList">包含在控件内的控件的集合</param>
        /// <param name="parent">控件的父容器</param>
        public static void MouseUpEvent(
            MouseEventArgs e,
            List<CharmControl> controlList,
            Control parent)
        {
            // 过程需要用到的变量
            List<Rectangle> rectRedrawList = new List<Rectangle>(); // 重绘区域集合

            // 轮询控件集合
            foreach (CharmControl control in controlList)
            {
                // 判断控件是否可见
                if (!control.Visible)
                    continue;
                // 判断控件类型并发生行为
                switch (control.ControlType)
                {
                    case ControlType.CharmSysButton:
                        if (control.ClientRectangle.Contains(e.Location))     // 判断是否在控件工作区内
                            switch (e.Button)   // 判断鼠标按钮
                            {
                                case MouseButtons.Left:
                                    if (control.ControlStatus != ControlStatus.Normal)    // 判断是否已经是常态
                                    {
                                        control.ControlStatus = ControlStatus.Normal;      // 设置控件状态
                                        control.TextPosition = new Point(control.TextPosition.X - 1, control.TextPosition.Y - 1);
                                        rectRedrawList.Add(control.ClientRectangle);         // 将控件工作区的矩形添加至重绘集合
                                        control.OnMouseUp(e);                                       // 触发控件事件
                                    }
                                    break;
                                case MouseButtons.Right:
                                    break;
                            }
                        break;
                    case ControlType.CharmButton:
                        if (control.ControlStatus == ControlStatus.Unenabled)   // 判断按钮是否为禁止态
                            break;
                        else
                            goto case ControlType.CharmSysButton;   // 非禁止态处理过程与系统按钮一致
                }
            }

            // 判断是否存在需要重绘的控件
            if (rectRedrawList.Count > 0)
            {
                Rectangle rectRedraw = rectRedrawList[0];
                for (int i = 1; i < rectRedrawList.Count; i++)
                    rectRedraw = Rectangle.Union(rectRedraw, rectRedrawList[i]);
                parent.Invalidate(rectRedraw);
            }
        }
        #endregion
    }
}