using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace UrlShortener.Tests
{
    /// <summary>
    /// Inline auto Moq data attribute to allow for inline auto fixture test models as well as Moq based items
    /// </summary>
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] values)
            : base(new AutoNSubstituteDataAttribute(), values)
        {
        }

        /// <summary>
        /// Private attribute which is required to be passed in. This sets up the <see cref="IFixture"/> to hook NSubstitute into the pipeline.
        /// </summary>
        private class AutoNSubstituteDataAttribute : AutoDataAttribute
        {
            public AutoNSubstituteDataAttribute()
                : base(() =>
                {
                    var fixture = new Fixture();
                    fixture.Customize(new CompositeCustomization(new AutoMoqCustomization()));
                    return fixture;
                })
            {
            }
        }
    }
}
