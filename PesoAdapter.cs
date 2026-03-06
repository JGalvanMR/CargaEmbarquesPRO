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
    public class PesoAdapter : BaseAdapter<PesoItem>
    {
        private readonly Activity _context;
        private readonly List<PesoItem> _items;

        public PesoAdapter(Activity context, List<PesoItem> items)
        {
            _context = context;
            _items = items;
        }

        public override PesoItem this[int position] => _items[position];

        public override int Count => _items.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            View view = convertView;

            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.item_peso, parent, false);
            }

            var txtName = view.FindViewById<TextView>(Resource.Id.txtName);
            var txtPeso = view.FindViewById<TextView>(Resource.Id.txtAge);
            var imgPers = view.FindViewById<ImageView>(Resource.Id.imgPers);

            txtName.Text = item.Nombre;
            txtPeso.Text = item.Peso;
            imgPers.SetImageResource(item.ImagenResourceId);

            return view;
        }
    }

}