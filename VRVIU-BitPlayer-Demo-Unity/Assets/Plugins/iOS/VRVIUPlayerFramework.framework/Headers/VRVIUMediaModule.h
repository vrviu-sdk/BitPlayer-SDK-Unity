/*
 * VRVIUMediaModule.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 *
 */

#import <Foundation/Foundation.h>

@interface VRVIUMediaModule : NSObject

+ (VRVIUMediaModule *)sharedModule;

@property(atomic, getter=isAppIdleTimerDisabled)            BOOL appIdleTimerDisabled;
@property(atomic, getter=isMediaModuleIdleTimerDisabled)    BOOL mediaModuleIdleTimerDisabled;

@end
