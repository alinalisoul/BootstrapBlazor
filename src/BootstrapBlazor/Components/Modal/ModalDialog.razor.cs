﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ModalDialog
    {
        private ElementReference DialogElement { get; set; }

        private JSInterop<ModalDialog>? Interop { get; set; }

        /// <summary>
        /// 获得 弹窗组件样式
        /// </summary>
        private string? ClassName => CssBuilder.Default("modal-dialog")
            .AddClass("modal-dialog-centered", IsCentered)
            .AddClass($"modal-{Size.ToDescriptionString()}", Size != Size.None)
            .AddClass("modal-dialog-scrollable", IsScrolling)
            .AddClass("is-draggable", IsDraggable)
            .AddClass("d-none", !IsShown)
            .Build();

        /// <summary>
        /// 获得/设置 是否显示对话框
        /// </summary>
        internal bool IsShown { get; set; }

        /// <summary>
        /// 获得/设置 弹窗标题
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// 获得/设置 弹窗大小
        /// </summary>
        [Parameter]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// 获得/设置 是否垂直居中 默认为 true
        /// </summary>
        [Parameter]
        public bool IsCentered { get; set; }

        /// <summary>
        /// 获得/设置 是否弹窗正文超长时滚动
        /// </summary>
        [Parameter]
        public bool IsScrolling { get; set; }

        /// <summary>
        /// 获得/设置 是否可以拖拽弹窗
        /// </summary>
        [Parameter]
        public bool IsDraggable { get; set; }

        /// <summary>
        /// 获得/设置 是否显示关闭按钮
        /// </summary>
        [Parameter]
        public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// 获得/设置 是否显示 Footer 默认为 true
        /// </summary>
        [Parameter]
        public bool ShowFooter { get; set; } = true;

        /// <summary>
        /// 获得/设置 弹窗内容相关数据 多用于传值
        /// </summary>
        [Parameter]
        public object? BodyContext { get; set; }

        /// <summary>
        /// 获得/设置 ModalBody 组件
        /// </summary>
        [Parameter]
        public RenderFragment? BodyTemplate { get; set; }

        /// <summary>
        /// 获得/设置 ModalFooter 组件
        /// </summary>
        [Parameter]
        public RenderFragment? FooterTemplate { get; set; }

        /// <summary>
        /// 获得/设置 关闭弹窗是回调委托
        /// </summary>
        [Parameter]
        [NotNull]
        public Func<Task>? OnClose { get; set; }

        /// <summary>
        /// 获得/设置 弹窗容器实例
        /// </summary>
        [CascadingParameter]
        [NotNull]
        public Modal? Modal { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (OnClose == null) OnClose = async () => await Modal.CloseOrPopDialog();

            Modal.AddDialog(this);
        }

        /// <summary>
        /// 显示弹窗方法
        /// </summary>
        public void Show() => Modal.ShowDialog(this);

        /// <summary>
        /// OnAfterRenderAsync 方法
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                Interop = new JSInterop<ModalDialog>(JSRuntime);
                await Interop.Invoke(this, DialogElement, "bb_modal_dialog", nameof(Close));
            }
        }

        private async Task OnClickClose()
        {
            Modal.RemoveDialog(this);
            if (OnClose != null)
            {
                await OnClose();
            }
        }

        /// <summary>
        /// Close 方法 客户端按 ESC 键盘时调用
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public Task Close() => OnClickClose();

        /// <summary>
        /// Dispose 方法
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Interop?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
