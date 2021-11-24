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
            driver.FindElement(By.XPath("//li[@class='container   '][1]/a")).Click();
            driver.FindElement(By.XPath("//li[@class='container    open']/ul/li[1]/a[@class='red']")).Click();
            driver.FindElement(By.XPath("//span[@id='bold-2']/span")).Click();
            driver.FindElement(By.XPath("//input[@id='filter_value_digital_min_2']")).Clear();
            driver.FindElement(By.XPath("//input[@id='filter_value_digital_min_2']")).SendKeys("1000");
            driver.FindElement(By.XPath("//input[@id='filter_value_digital_max_2']")).Clear();
            driver.FindElement(By.XPath("//input[@id='filter_value_digital_max_2']")).SendKeys("10000");

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
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//span[@id='basket_button_324861']/*"))).Build().Perform();
            Assert.AreEqual("Добавить в корзину", driver.FindElement(By.XPath("//span[@id='basket_button_324861']/a")).GetAttribute("title").Trim(),
                "Tooltip has not appeared.");
            //тултипы на данном сайте не являются css
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.XPath("//li[@class='persona ']/span")).Click();
            driver.FindElement(By.XPath("//div[1]/span[@id='persona_regShowButton']")).Click();
            driver.FindElement(By.XPath("//input[@id='new_password1']")).SendKeys("12345");
            driver.FindElement(By.XPath("//input[@id='persona_inputName']")).SendKeys("Vitaly");
            driver.FindElement(By.XPath("//input[@id='persona_inputPhone']")).SendKeys("89789234565");
            driver.FindElement(By.XPath("//button[@id='persona_regButton']")).Click();
            Assert.IsTrue(driver.FindElements(By.XPath("//div[@id='personaSubForm']/*[contains(@class, 'input_likeBootstrap requireField_notFilled')]")).Any(),
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
