﻿/*
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace TestSuite
{
   [TestClass()]
   public class T_Dates
   {
      [TestMethod()]
      public void testIMMDates()
      {
         // ("Testing IMM dates...");

          var settings = new SavedSettings();

         string[] IMMcodes = new string[] {
                "F0", "G0", "H0", "J0", "K0", "M0", "N0", "Q0", "U0", "V0", "X0", "Z0",
                "F1", "G1", "H1", "J1", "K1", "M1", "N1", "Q1", "U1", "V1", "X1", "Z1",
                "F2", "G2", "H2", "J2", "K2", "M2", "N2", "Q2", "U2", "V2", "X2", "Z2",
                "F3", "G3", "H3", "J3", "K3", "M3", "N3", "Q3", "U3", "V3", "X3", "Z3",
                "F4", "G4", "H4", "J4", "K4", "M4", "N4", "Q4", "U4", "V4", "X4", "Z4",
                "F5", "G5", "H5", "J5", "K5", "M5", "N5", "Q5", "U5", "V5", "X5", "Z5",
                "F6", "G6", "H6", "J6", "K6", "M6", "N6", "Q6", "U6", "V6", "X6", "Z6",
                "F7", "G7", "H7", "J7", "K7", "M7", "N7", "Q7", "U7", "V7", "X7", "Z7",
                "F8", "G8", "H8", "J8", "K8", "M8", "N8", "Q8", "U8", "V8", "X8", "Z8",
                "F9", "G9", "H9", "J9", "K9", "M9", "N9", "Q9", "U9", "V9", "X9", "Z9"
            };

         Date counter = Date.minDate();
         // 10 years of futures must not exceed Date::maxDate
         Date last = Date.maxDate() - new Period(121, TimeUnit.Months);
         Date imm;

         while (counter <= last)
         {
            imm = IMM.nextDate(counter, false, settings);

            // check that imm is greater than counter
            if (imm <= counter)
               Assert.Fail(imm.DayOfWeek + " " + imm
                          + " is not greater than "
                          + counter.DayOfWeek + " " + counter);

            // check that imm is an IMM date
            if (!IMM.isIMMdate(imm, false))
               Assert.Fail(imm.DayOfWeek + " " + imm
                          + " is not an IMM date (calculated from "
                          + counter.DayOfWeek + " " + counter + ")");

            // check that imm is <= to the next IMM date in the main cycle
            if (imm > IMM.nextDate(counter, true, settings))
               Assert.Fail(imm.DayOfWeek + " " + imm
                          + " is not less than or equal to the next future in the main cycle "
                          + IMM.nextDate(counter, true, settings));

            //// check that if counter is an IMM date, then imm==counter
            //if (IMM::isIMMdate(counter, false) && (imm!=counter))
            //    BOOST_FAIL("\n  "
            //               << counter.weekday() << " " << counter
            //               << " is already an IMM date, while nextIMM() returns "
            //               << imm.weekday() << " " << imm);

            // check that for every date IMMdate is the inverse of IMMcode
            if (IMM.date(IMM.code(imm), counter, settings) != imm)
               Assert.Fail(IMM.code(imm)
                          + " at calendar day " + counter
                          + " is not the IMM code matching " + imm);

            // check that for every date the 120 IMM codes refer to future dates
            for (int i = 0; i < 40; ++i)
            {
               if (IMM.date(IMMcodes[i], counter, settings) < counter)
                  Assert.Fail(IMM.date(IMMcodes[i], counter, settings)
                         + " is wrong for " + IMMcodes[i]
                         + " at reference date " + counter);
            }

            counter = counter + 1;
         }
      }

      [TestMethod()]
      public void testConsistency()
      {
         //("Testing dates...");

         int minDate = Date.minDate().serialNumber() + 1,
                    maxDate = Date.maxDate().serialNumber();

         int dyold = new Date(minDate - 1).DayOfYear,
             dold = new Date(minDate - 1).Day,
             mold = new Date(minDate - 1).Month,
             yold = new Date(minDate - 1).Year,
             wdold = new Date(minDate - 1).weekday();

         for (int i = minDate; i <= maxDate; i++)
         {
            Date t = new Date(i);
            int serial = t.serialNumber();

            // check serial number consistency
            if (serial != i)
               Assert.Fail("inconsistent serial number:\n"
                          + "    original:      " + i + "\n"
                          + "    date:          " + t + "\n"
                          + "    serial number: " + serial);

            int dy = t.DayOfYear,
                d = t.Day,
                m = t.Month,
                y = t.Year,
                wd = t.weekday();

            // check if skipping any date
            if (!((dy == dyold + 1) ||
                  (dy == 1 && dyold == 365 && !Date.IsLeapYear(yold)) ||
                  (dy == 1 && dyold == 366 && Date.IsLeapYear(yold))))
               Assert.Fail("wrong day of year increment: \n"
                          + "    date: " + t + "\n"
                          + "    day of year: " + dy + "\n"
                          + "    previous:    " + dyold);
            dyold = dy;

            if (!((d == dold + 1 && m == mold && y == yold) ||
                  (d == 1 && m == mold + 1 && y == yold) ||
                  (d == 1 && m == 1 && y == yold + 1)))
               Assert.Fail("wrong day,month,year increment: \n"
                          + "    date: " + t + "\n"
                          + "    day,month,year: "
                          + d + "," + m + "," + y + "\n"
                          + "    previous:       "
                          + dold + "," + mold + "," + yold);
            dold = d; mold = m; yold = y;

            // check month definition
            if (m < 1 || m > 12)
               Assert.Fail("invalid month: \n"
                          + "    date:  " + t + "\n"
                          + "    month: " + m);

            // check day definition
            if (d < 1)
               Assert.Fail("invalid day of month: \n"
                          + "    date:  " + t + "\n"
                          + "    day: " + d);
            if (!((m == 1 && d <= 31) ||
                  (m == 2 && d <= 28) ||
                  (m == 2 && d == 29 && Date.IsLeapYear(y)) ||
                  (m == 3 && d <= 31) ||
                  (m == 4 && d <= 30) ||
                  (m == 5 && d <= 31) ||
                  (m == 6 && d <= 30) ||
                  (m == 7 && d <= 31) ||
                  (m == 8 && d <= 31) ||
                  (m == 9 && d <= 30) ||
                  (m == 10 && d <= 31) ||
                  (m == 11 && d <= 30) ||
                  (m == 12 && d <= 31)))
               Assert.Fail("invalid day of month: \n"
                          + "    date:  " + t + "\n"
                          + "    day: " + d);

            // check weekday definition
            if (!((wd == wdold + 1) ||
                  (wd == 1 && wdold == 7)))
               Assert.Fail("invalid weekday: \n"
                          + "    date:  " + t + "\n"
                          + "    weekday:  " + wd + "\n"
                          + "    previous: " + wdold);
            wdold = wd;

            // create the same date with a different constructor
            Date s = new Date(d, m, y);
            // check serial number consistency
            serial = s.serialNumber();
            if (serial != i)
               Assert.Fail("inconsistent serial number:\n"
                          + "    date:          " + t + "\n"
                          + "    serial number: " + i + "\n"
                          + "    cloned date:   " + s + "\n"
                          + "    serial number: " + serial);
         }

      }
   }
}