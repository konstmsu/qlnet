/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
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

namespace QLNet {
    public static partial class Utils {
        public static BusinessDayConvention eurliborConvention(Period p) {
            switch (p.units()) {
                case TimeUnit.Days:
                case TimeUnit.Weeks:
                    return BusinessDayConvention.Following;
                case TimeUnit.Months:
                case TimeUnit.Years:
                    return BusinessDayConvention.ModifiedFollowing;
                default:
                    throw new ArgumentException("Unknown TimeUnit: " + p.units());
            }
        }

        public static bool eurliborEOM(Period p) {
            switch (p.units()) {
                case TimeUnit.Days:
                case TimeUnit.Weeks:
                    return false;
                case TimeUnit.Months:
                case TimeUnit.Years:
                    return true;
                default:
                    throw new ArgumentException("Unknown TimeUnit: " + p.units());
            }
        }
    }

    /// <summary>
    /// base class for all BBA %EUR %LIBOR indexes but the O/N
    ///    ! Euro LIBOR fixed by BBA.
    ///
    ///        See <href a="http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414" />.
    ///
    ///        \warning This is the rate fixed in London by BBA. Use Euribor if
    ///                 you're interested in the fixing by the ECB.
    /// </summary>
    public class EURLibor : IborIndex {
        private Calendar target_;

        // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
        // JoinBusinessDays is the fixing calendar for
        // all indexes but o/n

        public EURLibor(Period tenor, SavedSettings settings)
            : base("EURLibor", tenor, 2, new EURCurrency(), new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new TARGET(),
                JointCalendar.JointCalendarRule.JoinHolidays),
                Utils.eurliborConvention(tenor), Utils.eurliborEOM(tenor), new Actual360(),
                new Handle<YieldTermStructure>(), settings) {
            target_ = new TARGET();
            if (!(this.tenor().units() != TimeUnit.Days))
                throw new ApplicationException("for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
        }

        public EURLibor(Period tenor, Handle<YieldTermStructure> h, SavedSettings settings)
            : base("EURLibor", tenor, 2, new EURCurrency(), new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new TARGET(),
                JointCalendar.JointCalendarRule.JoinHolidays),
                Utils.eurliborConvention(tenor), Utils.eurliborEOM(tenor), new Actual360(), h, settings) {
            target_ = new TARGET();
            if (!(this.tenor().units() != TimeUnit.Days))
                throw new ApplicationException("for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
        }

        //        ! \name Date calculations
        //
        //            see http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412
        //            @{
        //        
        public override Date valueDate(Date fixingDate) {

            if (!(isValidFixingDate(fixingDate)))
                throw new ApplicationException("Fixing date " + fixingDate + " is not valid");

            // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
            // In the case of EUR the Value Date shall be two TARGET
            // business days after the Fixing Date.
            return target_.advance(fixingDate, fixingDays_, TimeUnit.Days);
        }
        public override Date maturityDate(Date valueDate) {
            // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
            // In the case of EUR only, maturity dates will be based on days in
            // which the Target system is open.
            return target_.advance(valueDate, tenor_, convention_, endOfMonth());
        }
    }

    //! base class for the one day deposit BBA %EUR %LIBOR indexes
    //    ! Euro O/N LIBOR fixed by BBA. It can be also used for T/N and S/N
    //        indexes, even if such indexes do not have BBA fixing.
    //
    //        See <http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414>.
    //
    //        \warning This is the rate fixed in London by BBA. Use Eonia if
    //                 you're interested in the fixing by the ECB.
    //    
    public class DailyTenorEURLibor : IborIndex {

        // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
        // no o/n or s/n fixings (as the case may be) will take place
        // when the principal centre of the currency concerned is
        // closed but London is open on the fixing day.
        public DailyTenorEURLibor(int settlementDays, SavedSettings settings)
            : this(settlementDays, new Handle<YieldTermStructure>(), settings) {
        }
        public DailyTenorEURLibor(SavedSettings settings)
            : base("EURLibor", new Period(1, TimeUnit.Days), 0, new EURCurrency(), new TARGET(),
            Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), new Handle<YieldTermStructure>(), settings) {
        }
        public DailyTenorEURLibor(int settlementDays, Handle<YieldTermStructure> h, SavedSettings settings)
            : base("EURLibor", new Period(1, TimeUnit.Days), settlementDays, new EURCurrency(), new TARGET(),
            Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), h, settings) {
        }
    }

    //! Overnight %EUR %Libor index
    public class EURLiborON : DailyTenorEURLibor {
        public EURLiborON(SavedSettings settings)
            : base(0, new Handle<YieldTermStructure>(), settings) {
        }

        public EURLiborON(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(0, h, settings) {
        }
    }

    //! 1-week %EUR %Libor index
    public class EURLiborSW : EURLibor {
        public EURLiborSW(SavedSettings settings)
            : base(new Period(1, TimeUnit.Weeks), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLiborSW(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(1, TimeUnit.Weeks), h, settings) {
        }
    }

    //! 2-weeks %EUR %Libor index
    public class EURLibor2W : EURLibor {
        public EURLibor2W(SavedSettings settings)
            : base(new Period(2, TimeUnit.Weeks), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor2W(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(2, TimeUnit.Weeks), h, settings) {
        }
    }


    //! 1-month %EUR %Libor index
    public class EURLibor1M : EURLibor {
        public EURLibor1M(SavedSettings settings)
            : base(new Period(1, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor1M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(1, TimeUnit.Months), h, settings) {
        }
    }

    //! 2-months %EUR %Libor index
    public class EURLibor2M : EURLibor {
        public EURLibor2M(SavedSettings settings)
            : base(new Period(2, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor2M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(2, TimeUnit.Months), h, settings) {
        }
    }

    //! 3-months %EUR %Libor index
    public class EURLibor3M : EURLibor {
        public EURLibor3M(SavedSettings settings)
            : base(new Period(3, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor3M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(3, TimeUnit.Months), h, settings) {
        }
    }

    //! 4-months %EUR %Libor index
    public class EURLibor4M : EURLibor {
        public EURLibor4M(SavedSettings settings)
            : base(new Period(4, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor4M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(4, TimeUnit.Months), h, settings) {
        }
    }

    //! 5-months %EUR %Libor index
    public class EURLibor5M : EURLibor {
        public EURLibor5M(SavedSettings settings)
            : base(new Period(5, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor5M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(5, TimeUnit.Months), h, settings) {
        }
    }

    //! 6-months %EUR %Libor index
    public class EURLibor6M : EURLibor {
        public EURLibor6M(SavedSettings settings)
            : base(new Period(6, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor6M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(6, TimeUnit.Months), h, settings) {
        }
    }

    //! 7-months %EUR %Libor index
    public class EURLibor7M : EURLibor {
        public EURLibor7M(SavedSettings settings)
            : base(new Period(7, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor7M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(7, TimeUnit.Months), h, settings) {
        }
    }

    //! 8-months %EUR %Libor index
    public class EURLibor8M : EURLibor {
        public EURLibor8M(SavedSettings settings)
            : base(new Period(8, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor8M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(8, TimeUnit.Months), h, settings) {
        }
    }

    //! 9-months %EUR %Libor index
    public class EURLibor9M : EURLibor {
        public EURLibor9M(SavedSettings settings)
            : base(new Period(9, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor9M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(9, TimeUnit.Months), h, settings) {
        }
    }

    //! 10-months %EUR %Libor index
    public class EURLibor10M : EURLibor {
        public EURLibor10M(SavedSettings settings)
            : base(new Period(10, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor10M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(10, TimeUnit.Months), h, settings) {
        }
    }

    //! 11-months %EUR %Libor index
    public class EURLibor11M : EURLibor {
        public EURLibor11M(SavedSettings settings)
            : base(new Period(11, TimeUnit.Months), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor11M(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(11, TimeUnit.Months), h, settings) {
        }
    }

    //! 1-year %EUR %Libor index
    public class EURLibor1Y : EURLibor {
        public EURLibor1Y(SavedSettings settings)
            : base(new Period(1, TimeUnit.Years), new Handle<YieldTermStructure>(), settings) {
        }
        public EURLibor1Y(Handle<YieldTermStructure> h, SavedSettings settings)
            : base(new Period(1, TimeUnit.Years), h, settings) {
        }
    }


}
