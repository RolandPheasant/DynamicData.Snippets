using System;
using System.Diagnostics;
using System.Linq;

namespace DynamicData.Snippets.Group
{
    public class CustomTotalRows: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservableCache<TradeProxy, AggregationKey> AggregatedData { get; }

        public CustomTotalRows(IObservableCache<Trade, int> source)
        {
            /*
                This technique can be used to create grid data with dynamic running totals 

                [In production systems I also include the ability to have total row expanders - perhaps an example could follow another time]
             */

            //1. create a trade proxy which enriches the trade with an aggregation id
            var tickers = source.Connect()
                .ChangeKey(proxy => new AggregationKey(AggregationType.Item, proxy.Id.ToString()))
                .Transform((trade, key) => new TradeProxy(trade, key));
                    

            //2. create grouping based on each ticker
            var tickerTotals = source.Connect()
                .GroupWithImmutableState(trade => new AggregationKey(AggregationType.SubTotal, trade.Ticker))
                .Transform(grouping => new TradeProxy(grouping.Items.ToArray(), grouping.Key));

            //3. create grouping of 1 so we can create grand total row
            var overallTotal = source.Connect()
                .GroupWithImmutableState(trade => new AggregationKey(AggregationType.GrandTotal, "All"))
                .Transform(grouping => new TradeProxy(grouping.Items.ToArray(), grouping.Key));
            
            //4. join all togther so results are in a single cache
            AggregatedData = tickers.Or(overallTotal)
                .Or(tickerTotals)
                .AsObservableCache();

            _cleanUp = AggregatedData;

        }
        
        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    [DebuggerDisplay("{Ticker} ({Key.Type})  Name={Value}")]
    public class TradeProxy
    {
        public AggregationKey Key { get; }
        public string Ticker { get; }
        public decimal Value { get; }

        public TradeProxy(Trade trade, AggregationKey key)
        {
            Key = key;
            Ticker = key.Value;
            Value = trade.Value;
        }
        
        public TradeProxy(Trade[] trades, AggregationKey key)
        {
            Key = key;
            Value = trades.Select(t => t.Value).Sum();
            Ticker = key.Value;
        }

    }

    public struct AggregationKey
    {
        public AggregationType Type { get; }
        public string Value { get; }

        public AggregationKey(AggregationType type, string value)
        {
            Type = type;
            Value = value;
        }

        #region Equality

        public bool Equals(AggregationKey other)
        {
            return Type == other.Type && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AggregationKey && Equals((AggregationKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return $"{Type} ({Value})";
        }
    }

    public enum AggregationType
    {
        Item,
        SubTotal,
        GrandTotal
    }

    public class Trade
    {
        public int Id { get; }
        public string Ticker { get; }
        public decimal Value { get; }


        public Trade(int id, string ticker, decimal value)
        {
            Id = id;
            Ticker = ticker;
            Value = value;
        }
    }
}
