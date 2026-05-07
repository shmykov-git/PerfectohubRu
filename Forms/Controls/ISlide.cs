using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace PerfectohubRu.Controls
{
    public interface ISlide
    {
        Visibility Visibility { get; set; }
        Task PlayEnterAnimation();
        Task PlayExitAnimation();
        //void ApplyServiceProvider(IServiceProvider sp);
    }
}
