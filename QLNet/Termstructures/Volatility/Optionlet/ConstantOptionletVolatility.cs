/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
 This file is part of QLNet Project http://qlnet.sourceforge.net/

 QLNet is free software: you can redistribute it and/or modify it
 under the terms of the QLNet license.  You should have received a
 copy of the license along with this program; if not, license is  
 available online at <http://qlnet.sourceforge.net/License.html>.
  
 QLNet is a based on QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {
    //! Constant caplet volatility, no time-strike dependence
    public class ConstantOptionletVolatility : OptionletVolatilityStructure {
        private Handle<Quote> volatility_;
        SavedSettings _settings;

        //! floating reference date, floating market data
        public ConstantOptionletVolatility(int settlementDays, Calendar cal, BusinessDayConvention bdc, Handle<Quote> vol, DayCounter dc, SavedSettings settings) 
            : base(settlementDays, cal, bdc, settings, dc) {
            volatility_ = vol;
            
            volatility_.registerWith(update);
            _settings = settings;
            }

        //! fixed reference date, floating market data
        public ConstantOptionletVolatility(Date referenceDate, Calendar cal, BusinessDayConvention bdc, Handle<Quote> vol, DayCounter dc, SavedSettings settings)
            : base(referenceDate, cal, bdc, settings, dc) {
            volatility_ = vol;
            
            volatility_.registerWith(update);
            _settings = settings;
            }

        //! floating reference date, fixed market data
        public ConstantOptionletVolatility(int settlementDays, Calendar cal, BusinessDayConvention bdc, double vol, DayCounter dc, SavedSettings settings)
            : base(settlementDays, cal, bdc, settings, dc)
        {
            volatility_ = new Handle<Quote>(new SimpleQuote(vol));
            _settings = settings;
        }

        //! fixed reference date, fixed market data
        public ConstantOptionletVolatility(Date referenceDate, Calendar cal, BusinessDayConvention bdc, double vol, DayCounter dc, SavedSettings settings)
            : base(referenceDate, cal, bdc, settings, dc)
        {
            volatility_ = new Handle<Quote>(new SimpleQuote(vol));
            _settings = settings;
        }


        public override Date maxDate() { return Date.maxDate(); }
        public override double minStrike() { return double.MinValue; }
        public override double maxStrike() { return double.MaxValue; }

        protected override SmileSection smileSectionImpl(Date d) {
            double atmVol = volatility_.link.value();
            return new FlatSmileSection(d, atmVol, dayCounter(), referenceDate(), _settings);
        }

        protected override SmileSection smileSectionImpl(double optionTime) {
            double atmVol = volatility_.link.value();
            return new FlatSmileSection(optionTime, atmVol, dayCounter(), _settings);
        }

        protected override double volatilityImpl(double d1, double d2) {
            return volatility_.link.value();
        }

    }
}
