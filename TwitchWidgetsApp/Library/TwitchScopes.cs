using System.Collections.Generic;
using Twitch.Base;

namespace TwitchWidgetsApp.Library;

public class TwitchScopes
{
        public static readonly List<OAuthClientScopeEnum> Scopes =
        [
            OAuthClientScopeEnum.channel_commercial,
            OAuthClientScopeEnum.channel_editor,
            OAuthClientScopeEnum.channel_read,
            OAuthClientScopeEnum.channel_subscriptions,

            OAuthClientScopeEnum.user_read,

            OAuthClientScopeEnum.bits__read,

            OAuthClientScopeEnum.channel__edit__commercial,

            OAuthClientScopeEnum.channel__manage__ads,
            OAuthClientScopeEnum.channel__manage__broadcast,
            OAuthClientScopeEnum.channel__manage__moderators,
            OAuthClientScopeEnum.channel__manage__polls,
            OAuthClientScopeEnum.channel__manage__predictions,
            OAuthClientScopeEnum.channel__manage__redemptions,
            OAuthClientScopeEnum.channel__manage__vips,

            OAuthClientScopeEnum.channel__moderate,

            OAuthClientScopeEnum.channel__read__ads,
            OAuthClientScopeEnum.channel__read__editors,
            OAuthClientScopeEnum.channel__read__goals,
            OAuthClientScopeEnum.channel__read__hype_train,
            OAuthClientScopeEnum.channel__read__polls,
            OAuthClientScopeEnum.channel__read__predictions,
            OAuthClientScopeEnum.channel__read__redemptions,
            OAuthClientScopeEnum.channel__read__subscriptions,
            OAuthClientScopeEnum.channel__read__vips,

            OAuthClientScopeEnum.clips__edit,

            OAuthClientScopeEnum.chat__edit,
            OAuthClientScopeEnum.chat__read,

            OAuthClientScopeEnum.moderation__read,

            OAuthClientScopeEnum.moderator__read__chat_settings,
            OAuthClientScopeEnum.moderator__read__followers,

            OAuthClientScopeEnum.moderator__manage__banned_users,
            OAuthClientScopeEnum.moderator__manage__chat_messages,
            OAuthClientScopeEnum.moderator__manage__chat_settings,
            OAuthClientScopeEnum.moderator__manage__shoutouts,

            OAuthClientScopeEnum.user__edit,

            OAuthClientScopeEnum.user__manage__blocked_users,
            OAuthClientScopeEnum.user__manage__whispers,

            OAuthClientScopeEnum.user__read__blocked_users,

            OAuthClientScopeEnum.user__read__broadcast,
            OAuthClientScopeEnum.user__read__follows,
            OAuthClientScopeEnum.user__read__subscriptions,

            OAuthClientScopeEnum.whispers__read,
            OAuthClientScopeEnum.whispers__edit
        ];
}