using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class RegardTests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.regard.ru/");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.XPath("//ul[contains(@class,'menu')]/li[contains(@class, 'container')][1]/a")).Click();
            driver.FindElement(By.XPath("//li[contains(@class, 'container')][contains(@class, 'open')]/ul/li[1]/a[@class='red']")).Click();
            driver.FindElement(By.XPath("//div[contains(@class, 'filter')]//span[contains(text(), 'Цена, руб')]")).Click();
            driver.FindElement(By.XPath("//div[@id='block_2']/input[contains(@id, 'filter_value_digital_min')]")).Clear();
            driver.FindElement(By.XPath("//div[@id='block_2']/input[contains(@id, 'filter_value_digital_min')]")).SendKeys("1000");
            driver.FindElement(By.XPath("//div[@id='block_2']/input[contains(@id, 'filter_value_digital_max')]")).Clear();
            driver.FindElement(By.XPath("//div[@id='block_2']/input[contains(@id, 'filter_value_digital_max')]")).SendKeys("10000");
            new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                .Until(x => driver.FindElements(By.XPath("//span[@id='finger']/a")).Any());
            driver.FindElement(By.XPath("//span[@id='finger']/a")).Click();

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'price')]/*[not(contains(@class,'basket_button_class'))]"))
               .Select(webPrice => webPrice.Text.Trim().Replace(" ", "")).ToArray<string>(), s => int.Parse(s));

            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 1000 && actualPrice <= 10000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 1000 and less or equal than 10000"));
        }

        [Test]
        public void TestTooltipText()
        {
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//span/a[contains(@class, 'cart')]"))).Build().Perform();
            Assert.AreEqual("Добавить в корзину", driver.FindElement(By.XPath("//span/a[contains(@class, 'cart')]")).GetAttribute("title").Trim(),
                "Tooltip has not appeared.");
            //тултипы на данном сайте не являются css
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.XPath("//span/span[@class='login']")).Click();
            driver.FindElement(By.XPath("//span[@id='persona_regShowButton']")).Click();
            driver.FindElement(By.XPath("//input[contains(@id, 'new_password')]")).SendKeys("12345");
            driver.FindElement(By.XPath("//input[@id='persona_inputName']")).SendKeys("Vitaly");
            driver.FindElement(By.XPath("//input[@id='persona_inputPhone']")).SendKeys("89789234565");
            driver.FindElement(By.XPath("//button[@id='persona_regButton']")).Click();
            Assert.IsTrue(driver.FindElements(By.XPath("//div[@id='personaSubForm']/*[contains(@class, 'input_likeBootstrap')][contains(@class, 'requireField_notFilled')]")).Any(),
                "E-mail confirmation button is enabled when e-mail input has no value.");
            //не пишу электронную почту, так как на данном сайте регистрация возможна без номера телефона
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}