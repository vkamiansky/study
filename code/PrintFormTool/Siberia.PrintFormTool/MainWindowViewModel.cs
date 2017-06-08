using System;
using System.Collections.Generic;
using System.Linq;

using Property;
using Property.Windows;

using Siberia.PrintFormTool.Entities;

namespace Siberia.PrintFormTool
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            var delayedIntegerInputPrototype = Reloadable<int>.On().Delayed(TimeSpan.FromMilliseconds(500)).Input();
            localHeight = delayedIntegerInputPrototype.Create();
            localWidth = delayedIntegerInputPrototype.Create();
            localTop = delayedIntegerInputPrototype.Create();
            localLeft = delayedIntegerInputPrototype.Create();

            localTextAreas = Reloadable<Dictionary<TextAreaEnum, string>>.On().First().Get(x =>
                new Dictionary<TextAreaEnum, string>()
                {
                    { TextAreaEnum.Area1, "Text Area 1" },
                    { TextAreaEnum.Area2, "Text Area 2" }
                }).Create();

            localSelectedTextArea = Reloadable<KeyValuePair<TextAreaEnum, string>>.On().Each().Input()
                .Create(localTextAreas.Value.First());

            localRectangles = Reloadable<Dictionary<TextAreaEnum, Rectangle>>.On().Each().Call(x =>
                {
                    x[localSelectedTextArea.Value.Key] = new Rectangle(localHeight.Value, localWidth.Value, localTop.Value, localLeft.Value);
                    return x;
                })
                .Create(new Dictionary<TextAreaEnum, Rectangle>() { { localTextAreas.Value.First().Key, new Rectangle(100, 100, 100, 100) } });

            localSelectedTextArea.OnChanged(() =>
            {
                var key = localSelectedTextArea.Value.Key;
                if (localRectangles.Value.ContainsKey(key))
                {
                    SetBox(localRectangles.Value[key]);
                }
            });

            localHeight.OnChanged(() => localRectangles);
            localWidth.OnChanged(() => localRectangles);
            localTop.OnChanged(() => localRectangles);
            localLeft.OnChanged(() => localRectangles);
        }

        private void SetBox(Rectangle rectangle)
        {
            localHeight.Input = rectangle.Height;
            localWidth.Input = rectangle.Width;
            localTop.Input = rectangle.Top;
            localLeft.Input = rectangle.Left;
        }

        private readonly IInputProperty<int> localHeight;

        private readonly IInputProperty<int> localWidth;

        private readonly IInputProperty<int> localTop;

        private readonly IInputProperty<int> localLeft;

        private readonly IProperty<Dictionary<TextAreaEnum, string>> localTextAreas;

        private readonly IInputProperty<KeyValuePair<TextAreaEnum, string>> localSelectedTextArea;

        private readonly ICallProperty<Dictionary<TextAreaEnum, Rectangle>> localRectangles;

        public IInputProperty<int> Height
        {
            get { return localHeight; }
        }

        public IInputProperty<int> Width
        {
            get { return localWidth; }
        }

        public IInputProperty<int> Top
        {
            get { return localTop; }
        }

        public IInputProperty<int> Left
        {
            get { return localLeft; }
        }

        public IProperty<Dictionary<TextAreaEnum, string>> TextAreas
        {
            get { return localTextAreas; }
        }

        public IInputProperty<KeyValuePair<TextAreaEnum, string>> SelectedTextArea
        {
            get { return localSelectedTextArea; }
        }
    }
}
