/*
 Copyright (C) 2008, 2009 , 2010, 2011, 2012  Andrea Maggiulli (a.maggiulli@gmail.com)
  
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

namespace QLNet
{
   public class BondFactory
   {
      public static AmortizingBond makeAmortizingBond(double FaceValue, double MarketValue, double CouponRate, Date IssueDate, Date MaturityDate, Date TradeDate, Frequency payFrequency, DayCounter dCounter, AmortizingMethod Method, SavedSettings settings, double gYield = 0)
      {
         return new AmortizingBond(FaceValue,
                                   MarketValue,
                                   CouponRate,
                                   IssueDate,
                                   MaturityDate,
                                   TradeDate,
                                   payFrequency,
                                   dCounter,
                                   Method,
                                   new NullCalendar(), settings, gYield);
      }


       public static AmortizingFixedRateBond makeAmortizingFixedBond(Date startDate, Period bondLength, DayCounter dCounter, Frequency payFrequency, double amount, double rate, SavedSettings settings)
       {
          return makeAmortizingFixedBond(startDate, bondLength, dCounter, payFrequency, amount, rate, new TARGET(), settings);
       }
          

      public static AmortizingFixedRateBond makeAmortizingFixedBond(Date startDate, Period bondLength, DayCounter dCounter, Frequency payFrequency, double amount, double rate, Calendar calendar, SavedSettings settings)
      {
         AmortizingFixedRateBond bond;
         Date endDate = calendar.advance(startDate,bondLength);

         Schedule schedule = new Schedule(startDate,endDate,bondLength,calendar,BusinessDayConvention.Unadjusted,
                                          BusinessDayConvention.Unadjusted,DateGeneration.Rule.Backward,false);

         bond = new AmortizingFixedRateBond(0, calendar, amount, startDate, bondLength, payFrequency, rate, dCounter, settings);

         return bond;

      }

      public static MBSFixedRateBond makeMBSFixedBond(Date startDate, Period bondLength, Period originalLength, DayCounter dCounter, Frequency payFrequency, double amount, double WACRate, double PassThroughRate, PSACurve psaCurve, SavedSettings settings)
      {
         return makeMBSFixedBond(startDate, bondLength, originalLength , dCounter, payFrequency, amount, WACRate, PassThroughRate, psaCurve, new TARGET(), settings);
      }


      public static MBSFixedRateBond makeMBSFixedBond(Date startDate, Period bondLength, Period originalLength, DayCounter dCounter, Frequency payFrequency, double amount, double WACrate, double PassThroughRate, PSACurve psaCurve, Calendar calendar, SavedSettings settings)
      {
         MBSFixedRateBond bond;
         Date endDate = calendar.advance(startDate, bondLength);

         Schedule schedule = new Schedule(startDate, endDate, bondLength, calendar, BusinessDayConvention.Unadjusted,
                                          BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);

         bond = new MBSFixedRateBond(0, calendar, amount, startDate, bondLength, originalLength , payFrequency, WACrate, PassThroughRate, dCounter, psaCurve, settings);

         return bond;

      }
   }
}
