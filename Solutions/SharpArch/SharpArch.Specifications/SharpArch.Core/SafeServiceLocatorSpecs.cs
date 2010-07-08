#pragma warning disable 169
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace SharpArch.Specifications.SharpArch.Core
{
    using System;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using CommonServiceLocator.WindsorAdapter;

    using global::SharpArch.Core;
    using global::SharpArch.Core.CommonValidator;
    using global::SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

    using Machine.Specifications;

    using Microsoft.Practices.ServiceLocation;

    using Rhino.Mocks;

    public class specification_for_safe_service_locator
    {
        Establish context = () => ServiceLocator.SetLocatorProvider(null);
    }

    [Subject(typeof(SafeServiceLocator<>))]
    public class when_the_safe_service_locator_is_asked_for_a_service : specification_for_safe_service_locator
    {
        static IValidator result;

        Establish context = () =>
        {
            var validator = MockRepository.GenerateMock<IValidator>();
            IWindsorContainer container = new WindsorContainer();
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
            container.Register(Component.For<IValidator>().Instance(validator));
        };

        Because of = () => result = SafeServiceLocator<IValidator>.GetService();

        It should_return_the_service = () => result.ShouldNotBeNull();
    }

    [Subject(typeof(SafeServiceLocator<>))]
    public class when_the_safe_service_locator_is_asked_for_a_service_but_has_not_been_initialized : specification_for_safe_service_locator
    {
        static Exception result;

        Because of = () => result = Catch.Exception(() => SafeServiceLocator<IValidator>.GetService());

        It should_throw_a_null_reference_exception = () => result.ShouldBeOfType<NullReferenceException>();
    }

    [Subject(typeof(SafeServiceLocator<>))]
    public class when_the_safe_service_locator_is_asked_for_a_service_that_has_not_been_registered : specification_for_safe_service_locator
    {
        static Exception result;

        Establish context = () =>
            {
                IWindsorContainer container = new WindsorContainer();
                ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
            };

        Because of = () => result = Catch.Exception(() => SafeServiceLocator<IValidator>.GetService());

        It should_throw_an_activation_exception = () => result.ShouldBeOfType<ActivationException>();
    }
}