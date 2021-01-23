using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SugorokuLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage
    {
        public ResultPage(IReadOnlyCollection<int> ranking, int myId, IReadOnlyCollection<Player> players)
        {
            InitializeComponent();
            StandImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.hyoushoudai.png");

            // 横幅相対 (コマ/表彰台) 横: 0.22, 縦: 0.68
            foreach (var (rank, playerId) in ranking.Select((id, index) => (index, id)))
            {
                var nowPlayer = players.First(p => p.PlayerID == playerId);
                var resource = $"SugorokuClientApp.ImageResource.koma_{playerId}.png";
                switch (rank)
                {
                    case 0:
                        TopKoma.Source = ImageSource.FromResource(resource);
                        TopLabel.Text = nowPlayer.PlayerName;
                        break;
                    case 1:
                        SecondKoma.Source = ImageSource.FromResource(resource);
                        SecondLabel.Text = nowPlayer.PlayerName;
                        break;
                    case 2:
                        ThirdKoma.Source = ImageSource.FromResource(resource);
                        ThirdLabel.Text = nowPlayer.PlayerName;
                        break;
                    case 3:
                        ForthKoma.Source = ImageSource.FromResource(resource);
                        ForthLabel.Text = nowPlayer.PlayerName;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            Device.BeginInvokeOnMainThread(async () => await FadeLayout(ranking.Count));
        }

        private async Task FadeLayout(int playerSize)
        {
            await TopLayout.FadeTo(1, 500);
            if (playerSize == 1) return;
            await SecondLayout.FadeTo(1, 500);
            if (playerSize == 2) return;
            await ThirdLayout.FadeTo(1, 500);
            if (playerSize == 3) return;
            await ForthLayout.FadeTo(1, 500);
        }
    }
}