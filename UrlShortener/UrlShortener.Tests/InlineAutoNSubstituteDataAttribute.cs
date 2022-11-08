using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace UrlShortener.Tests
{
    /// <summary>
    /// Inline auto NSubstitute data attribute to allow for inline auto fixture test models as well as NSubstitute based items
    /// </summary>
    public class InlineAutoNSubstituteDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
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
                    fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
                    return fixture;
                })
            {
            }
        }
    }
}
