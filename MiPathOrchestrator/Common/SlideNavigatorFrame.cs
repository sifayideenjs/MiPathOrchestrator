using System;

namespace MiPathOrchestrator.Common
{
    internal class SlideNavigatorFrame
    {
        public SlideNavigatorFrame(int slideIndex, Action setupSlide)
        {
            SlideIndex = slideIndex;
            SetupSlide = setupSlide;
        }

        public int SlideIndex { get; }

        public Action SetupSlide { get; }
    }
}
