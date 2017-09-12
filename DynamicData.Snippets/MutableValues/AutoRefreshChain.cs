using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace DynamicData.Snippets.MutableValues
{
    public class AutoRefreshChain
    {
    }

    public class Grade :AbstractNotifyPropertyChanged
    {
        private string _name;
        private int _score;
        private string _class;

        public Grade(int id, string name, string @class)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name
        {
            get => _name;
            set => SetAndRaise(ref _name, value);
        }

        public string Class
        {
            get => _class;
            set => SetAndRaise(ref _class, value);
        }

        public int Score
        {
            get => _score;
            set => SetAndRaise(ref _score, value);
        }
    }
}
