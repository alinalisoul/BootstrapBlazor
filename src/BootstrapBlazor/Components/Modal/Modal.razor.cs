﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class Modal
    {
        /// <summary>
        /// 获得/设置 DOM 元素实例
        /// </summary>
        private ElementReference ModalElement { get; set; }

        /// <summary>
        /// 获得 样式字符串
        /// </summary>
        private string? ClassString => CssBuilder.Default("modal")
            .AddClass("fade", IsFade)
            .Build();

        /// <summary>
        /// 获得 ModalDialog 集合
        /// </summary>
        private List<ModalDialog> Dialogs { get; set; } = new(8);

        /// <summary>
        /// 获得/设置 是否后台关闭弹窗
        /// </summary>
        [Parameter]
        public bool IsBackdrop { get; set; }

        /// <summary>
        /// 获得/设置 是否开启淡入淡出动画 默认为 true 开启动画
        /// </summary>
        [Parameter]
        public bool IsFade { get; set; } = true;

        /// <summary>
        /// 获得/设置 子组件
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 获得 后台关闭弹窗设置
        /// </summary>
        private string? Backdrop => IsBackdrop ? null : "static";

        /// <summary>
        /// 添加对话框方法
        /// </summary>
        /// <param name="dialog"></param>
        internal void AddDialog(ModalDialog dialog)
        {
            if (!Dialogs.Any())
            {
                dialog.IsShown = true;
            }

            Dialogs.Add(dialog);
        }

        /// <summary>
        /// 移除对话框方法
        /// </summary>
        /// <param name="dialog"></param>
        internal void RemoveDialog(ModalDialog? dialog = null)
        {
            if (dialog == null)
            {
                dialog = Dialogs.Last();
                dialog.Close();
            }
            else
            {
                Dialogs.Remove(dialog);
            }
        }

        /// <summary>
        /// 显示指定对话框方法
        /// </summary>
        /// <param name="dialog"></param>
        internal void ShowDialog(ModalDialog? dialog = null)
        {
            if (Dialogs.Any())
            {
                dialog ??= Dialogs.Last();
                Dialogs.ForEach(d => d.IsShown = d == dialog);
                StateHasChanged();
            }
        }

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
                await JSRuntime.InvokeVoidAsync(ModalElement, "bb_modal", "init");
            }
        }

        /// <summary>
        /// 弹窗状态切换方法
        /// </summary>
        public async ValueTask Toggle()
        {
            Dialogs.ForEach(d => d.IsShown = Dialogs.IndexOf(d) == 0);
            await JSRuntime.InvokeVoidAsync(ModalElement, "bb_modal", "toggle");
        }

        /// <summary>
        /// 显示弹窗方法
        /// </summary>
        /// <returns></returns>
        public async ValueTask Show()
        {
            var dialog = Dialogs.Last();
            Dialogs.ForEach(d => d.IsShown = dialog == d);
            await JSRuntime.InvokeVoidAsync(ModalElement, "bb_modal", "show");
        }

        /// <summary>
        /// 关闭当前弹窗方法
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            var dialog = Dialogs.FirstOrDefault(d => d.IsShown);
            if (dialog != null)
            {
                await dialog.Close();
            }
        }

        /// <summary>
        /// 内部使用如果还有弹窗继续显示，如果没有弹窗关闭所有
        /// </summary>
        /// <returns></returns>
        internal async ValueTask CloseOrPopDialog()
        {
            if (Dialogs.Any())
            {
                ShowDialog();
            }
            else
            {
                // 全部关闭
                await JSRuntime.InvokeVoidAsync(ModalElement, "bb_modal", "hide");
            }
        }
    }
}
