﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Fotootof.Layouts.Dialogs;
using Fotootof.SQLite.EntityManager.Managers;
using Fotootof.SQLite.Services;
using XtrmAddons.Net.Application;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Layouts.Forms.User
{
    /// <summary>
    /// <para>Class XtrmAddons Net Windows Validation Rule String Required.</para>
    /// <para>Check if a formated string as email is valid and not null, empty or whitespace.</para>
    /// </summary>
    internal class StringUniqueEmail : ValidationRule
    {
        #region Variables

        /// <summary>
        /// Variable logger <see cref="log4net.ILog"/>.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Properties
        
        /// <summary>
        /// Property wrapper object dependencies.
        /// </summary>
        public Wrapper Wrapper { get; set; }

        #endregion



        /// <summary>
        /// Method to validate rule to apply to the object.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <param name="cultureInfo">The culture informations.</param>
        /// <returns>True if conditions are validated otherwise false.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            if (!str.IsValidEmail())
            {
                return new ValidationResult(false, "This email is not not valid.");
            }

            if (!IsUniqueEmail(Wrapper.PrimaryKey, str))
            {
                return new ValidationResult(false, "This email is already registered.");
            }

            //return ValidationResult.ValidResult;
            return new ValidationResult(true, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="email"></param>
        public bool IsUniqueEmail(int pk, string email)
        {
            try
            {
                var usr = SQLiteSvc.GetInstance().Users.SingleOrNull(new UserOptionsSelect { Email = email });

                if (usr == null)
                {
                    return true;
                }
                else if (usr.PrimaryKey == pk)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Output());
                MessageBoxs.Error(ex.Output());
            }

            return false;
        }
    }

    /// <summary>
    /// <para>Class XtrmAddons Net Windows Validation Rule String Required.</para>
    /// <para>Check if a formated string as email is valid and not null, empty or whitespace.</para>
    /// </summary>
    internal class StringUniqueName : ValidationRule
    {
        #region Variables

        /// <summary>
        /// Variable logger <see cref="log4net.ILog"/>.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Properties

        /// <summary>
        /// Property wrapper object dependencies.
        /// </summary>
        public Wrapper Wrapper { get; set; }

        #endregion

        /// <summary>
        /// Method to validate rule to apply to the object.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <param name="cultureInfo">The culture informations.</param>
        /// <returns>True if conditions are validated otherwise false.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            if (str.IsNullOrWhiteSpace() || str.Length < 3)
            {
                return new ValidationResult(false, "The name must not be null, empty or whitespace.");
            }

            if (!IsUniqueName(Wrapper.PrimaryKey, str))
            {
                return new ValidationResult(false, "This name is already registered.");
            }

            //return ValidationResult.ValidResult;
            return new ValidationResult(true, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="name"></param>
        public bool IsUniqueName(int pk, string name)
        {
            try
            { 
                var u = SQLiteSvc.GetInstance().Users.SingleOrNull(new UserOptionsSelect { Name = name });

                if (u == null)
                {
                    return true;
                }
                else if (u.PrimaryKey == pk)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Output());
                MessageBoxs.Error(ex.Output());
            }

            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Wrapper : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PrimaryKeyProperty =
             DependencyProperty.Register("PrimaryKey", typeof(int),
             typeof(Wrapper), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NameProperty =
             DependencyProperty.Register("Name", typeof(string),
             typeof(Wrapper), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public int PrimaryKey
        {
            get { return (int)GetValue(PrimaryKeyProperty); }
            set { SetValue(PrimaryKeyProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    internal class BindingProxy : Freezable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// 
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}