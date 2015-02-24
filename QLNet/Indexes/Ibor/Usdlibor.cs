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

namespace QLNet {
    //! %USD %LIBOR rate
    /*! US Dollar LIBOR fixed by BBA.

        See <http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414>.
    */
    public class USDLibor : Libor {
        public USDLibor(Period tenor, SavedSettings settings) : this(tenor, new Handle<YieldTermStructure>(), settings) { }
        public USDLibor(Period tenor, Handle<YieldTermStructure> h, SavedSettings settings)
            : base("USDLibor", tenor, 2, new USDCurrency(), new UnitedStates(UnitedStates.Market.Settlement), new Actual360(), h, settings) { }
    }

    //! base class for the one day deposit BBA %USD %LIBOR indexes
    public class DailyTenorUSDLibor : DailyTenorLibor {
        public DailyTenorUSDLibor(int settlementDays, SavedSettings settings) : this(settlementDays, new Handle<YieldTermStructure>(), settings) {}
        public DailyTenorUSDLibor(int settlementDays, Handle<YieldTermStructure> h, SavedSettings settings)
            : base("USDLibor", settlementDays, new USDCurrency(), new UnitedStates(UnitedStates.Market.Settlement), new Actual360(), h, settings) {}
    };

    //! Overnight %USD %Libor index
    public class USDLiborON : DailyTenorUSDLibor {
        public USDLiborON(SavedSettings settings) : this(new Handle<YieldTermStructure>(), settings) { }
        public USDLiborON(Handle<YieldTermStructure> h, SavedSettings settings) : base(0, h, settings) {}
    }
}
