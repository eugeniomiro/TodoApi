using Microsoft.VisualStudio.TestTools.UnitTesting;

// see: https://saintgimp.org/2009/01/20/bdd-with-mstest/
namespace SimpleSpec.Bdd
{
    public class ContextSpecification
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Context();
            BecauseOf();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            CleanUp();
        }

        protected virtual void Context() { }
        protected virtual void BecauseOf() { }
        protected virtual void CleanUp() { }
    }
}