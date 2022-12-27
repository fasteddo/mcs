// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;


namespace mame
{
    public static partial class util
    {
        /// \brief A subscription to a notification source
        ///
        /// Class for representing a subscription to a notifier.  Automatically
        /// unsubscribes on destruction, if explicitly reset, if assigned to, or
        /// when taking ownership of a different subscription by move
        /// assignment.
        /// \sa notifier
        public class notifier_subscription
        {
            int m_token;  //std::weak_ptr<int> m_token;
            std.vector<bool> m_live;
            size_t m_index;


            /// \brief Create an empty subscription
            ///
            /// Initialises an instance not referring to a subscription.
            protected notifier_subscription() { m_token = 0; m_live = null; m_index = 0U; }

            /// \brief Transfer ownership of a subscription
            ///
            /// Transfers ownership of a subscription to a new instance.
            /// \param [in,out] that The subscription to transfer ownership
            ///   from.  Will no longer refer to a subscription after ownership
            ///   is transferred away.
            //notifier_subscription(notifier_subscription &&that) noexcept :
            //    m_token(std::move(that.m_token)),
            //    m_live(that.m_live),
            //    m_index(that.m_index)
            //{
            //    that.m_token.reset();
            //}

            protected notifier_subscription(
                    int token,  //std::shared_ptr<int> const &token,
                    std.vector<bool> live,
                    size_t index)  //std::vector<bool>::size_type index) :
            {
                m_token = token;
                m_live = live;
                m_index = index;
            }

            //notifier_subscription(notifier_subscription const &) = delete;
            //notifier_subscription &operator=(notifier_subscription const &) = delete;


            /// \brief Unsubscribe and destroy a subscription
            ///
            /// Unsubscribes if the subscription is active and cleans up the
            /// subscription instance.
            //~notifier_subscription() noexcept
            //{
            //    auto token(m_token.lock());
            //    if (token)
            //        (*m_live)[m_index] = false;
            //}


            /// \brief Swap two subscriptions
            ///
            /// Exchanges ownership of subscriptions between two instances.
            /// \param [in,out] that The subscription to exchange ownership
            ///   with.
            //void swap(notifier_subscription &that) noexcept

            /// \brief Unsubscribe from notifications
            ///
            /// If the instance refers to an active subscription, cancel it so
            /// no future notifications will be received.
            //void reset() noexcept

            /// \brief Test whether a subscription is active
            ///
            /// Tests whether a subscription is active.  A subscription will be
            /// inactive if it is default constructed, reset, transferred away,
            /// or if the underlying notifier is destructed.
            /// \return True if the subscription is active, false otherwise.
            //explicit operator bool() const noexcept { return !m_token.expired(); }

            /// \brief Transfer ownership of a subscription
            ///
            /// Transfers ownership of a subscription to an existing instance.
            /// If the subscription is active, it will be cancelled before it
            /// takes ownership of the other subscription.
            /// \param [in,out] that The subscription to transfer ownership
            ///   from.  Will no longer refer to a subscription after ownership
            ///   is transferred away.
            /// \return A reference to the instance that ownership was
            ///   transferred to.
            //notifier_subscription &operator=(notifier_subscription &&that) noexcept
        }


        /// \brief Broadcast notifier
        ///
        /// Calls multiple listener functions.  Allows listeners to subscribe
        /// and unsubscribe.  Subscriptions are managed using
        /// \c notifier_subscription instances.
        /// \tparam Params Argument types for the listener functions.
        /// \sa notifier_subscription
        //template <typename... Params>
        public class notifier<Params>
        {
            /// \brief Listener delegate type
            ///
            /// The delegate type used to represent listener functions.
            //using delegate_type = delegate<void (Params...)>;


            int m_token;  //std::shared_ptr<int> m_token;
            std.vector<bool> m_live = new std.vector<bool>();
            std.vector<Action<Params>> m_listeners = new std.vector<Action<Params>>();  //std::vector<delegate_type> m_listeners;


            /// \brief Create a new notifier
            ///
            /// Creates a new notifier with no initial subscribers.
            public notifier() { m_token = 0; }


            //notifier(notifier const &) = delete;
            //notifier &operator=(notifier const &) = delete;


            /// \brief Destroy a notifier
            ///
            /// Destroys a notifier, causing any subscriptions to become
            /// inactive.
            //~notifier() noexcept { m_token.reset(); }


            /// \brief Add a listener
            ///
            /// Adds a listener function subscription, returning an object for
            /// managing the subscription.
            /// \param [in] listener The function to be called on notifications.
            /// \return A subscription object.  Destroy the object or call its
            ///   \c reset member function to unsubscribe.

            class subscribe_subscription_impl : notifier_subscription
            {
                public subscribe_subscription_impl(notifier<Params> host, size_t index) :
                    base(host.m_token, host.m_live, index)
                {
                }
            }

            public notifier_subscription subscribe(Action<Params> listener)  //notifier_subscription subscribe(delegate_type &&listener)
            {
                for (size_t i = 0U; m_listeners.size() > i; ++i)
                {
                    if (!m_live[i])
                    {
                        m_live[i] = true;
                        m_listeners[i] = listener;  //m_listeners[i] = std::move(listener);
                        return new subscribe_subscription_impl(this, i);
                    }
                }

                m_live.emplace_back(true);
                try
                {
                    m_listeners.emplace_back(listener);  //m_listeners.emplace_back(std::move(listener));
                }
                catch (Exception)
                {
                    m_live.pop_back();
                    throw;
                }

                return new subscribe_subscription_impl(this, m_listeners.size() - 1);
            }


            /// \brief Call listeners
            ///
            /// Calls all active listener functions.
            /// \param [in] args Arguments to pass to the listener functions.
            public void op(Params args)  //void operator()(Params... args) const
            {
                for (size_t i = 0U; m_listeners.size() > i; ++i)
                {
                    if (m_live[i])
                        m_listeners[i](args);
                }
            }
        }


        /// \brief Swap two notifier subscriptions
        ///
        /// Exchanges ownership of two notifier subscriptions.  Allows the
        /// swappable idiom to be used with notifier subscriptions.
        /// \param [in,out] x Takes ownership of the subscription from \p y.
        /// \param [in,out] y Takes ownership of the subscription from \p x.
        /// \sa notifier_subscription
        //inline void swap(notifier_subscription &x, notifier_subscription &y) noexcept { x.swap(y); }
    }
}
