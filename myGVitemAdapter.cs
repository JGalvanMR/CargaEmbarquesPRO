using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CargaEmbarques.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CargaEmbarques
{
    public class myGVitemAdapter : BaseAdapter<FlimStarInfo>
    {
        Activity _CurrentContext;
        List<FlimStarInfo> _lstFlimStarInfo;

        public myGVitemAdapter(Activity currentContext, List<FlimStarInfo> lstFlimInfo)
        {
            _CurrentContext = currentContext;
            _lstFlimStarInfo = lstFlimInfo;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var item = _lstFlimStarInfo[position];
                if (convertView == null)
                    convertView = _CurrentContext.LayoutInflater.Inflate(Resource.Layout.custGridViewItem, null);

                convertView.FindViewById<TextView>(Resource.Id.txtName).Text = item.Name;
                convertView.FindViewById<TextView>(Resource.Id.txtAge).Text = item.Age.ToString();
                convertView.FindViewById<ImageView>(Resource.Id.imgPers).SetImageResource(item.ImageID);


            }
            catch (Exception e)
            {

                var error = e;
            }


            return convertView;
        }

        public override int Count
        {
            get { return _lstFlimStarInfo == null ? -1 : _lstFlimStarInfo.Count; }
        }

        public override FlimStarInfo this[int position] => _lstFlimStarInfo == null ? null : _lstFlimStarInfo[position];
    }
}