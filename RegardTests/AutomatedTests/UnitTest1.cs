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
    public class Tests
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
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@id='lsidebar']/div[@id='lmenu']/ul[@class='menu ']/li[@class='container   '][1]/a")).Click();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@id='lsidebar']/div[@id='lmenu']/ul[@class='menu ']/li[@class='container    open']/ul/li[1]/a[@class='red']")).Click();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/span[@id='bold-2']/span")).Click();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/div[@id='block_2']/input[@id='filter_value_digital_min_2']")).Clear();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/div[@id='block_2']/input[@id='filter_value_digital_min_2']")).SendKeys("1000");
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/div[@id='block_2']/input[@id='filter_value_digital_max_2']")).Clear();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/div[@id='block_2']/input[@id='filter_value_digital_max_2']")).SendKeys("10000");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='rsidebar']/div[@class='quick-order filter']/div[@id='content_filter']/a[@id='b_apply_filter']")).Click();

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'price')]/*[not(contains(@class,'basket_button_class'))]"))
               .Select(webPrice => webPrice.Text.Trim().Replace(" ", "")).ToArray<string>(), s => int.Parse(s));

            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 1000 && actualPrice <= 10000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 1000 and less or equal than 10000"));
        }

        [Test]
        public void TestTooltipText()
        {
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='hits']/div[@class='content']/div[@class='block'][1]/div[@class='bcontent']/div[@class='price']/span[@id='basket_button_324861']/*"))).Build().Perform();
            Assert.IsTrue(driver.FindElements(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='content']/div[@class='ccontent']/div[@class='cleft']/div[@id='hits']/div[@class='content']/div[@class='block'][1]/div[@class='bcontent']/div[@class='price']/span[@id='basket_button_324861']/a[@title='Добавить в корзину']")).Any(),
              "Tooltip has not appeared.");
            //тултипы на данном сайте не являются css
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/li[@class='persona ']/span")).Click();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/div[@id='personaSubForm']/div[1]/span[@id='persona_regShowButton']")).Click();
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/div[@id='personaSubForm']/input[@id='new_password1']")).SendKeys("12345");
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/div[@id='personaSubForm']/input[@id='persona_inputName']")).SendKeys("Vitaly");
            driver.FindElement(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/div[@id='personaSubForm']/button[@id='persona_regButton']")).Click();
            Assert.IsTrue(driver.FindElements(By.XPath("/html/body/div[@id='page']/div[@id='page_right']/div[@id='wrapper']/div[@id='header']/div[@id='directions']/ul[@id='main-menu']/div[@id='personaSubForm']/*[contains(@class, 'input_likeBootstrap requireField_notFilled')]")).Any(),
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
