/*
 * VRVIUNotificationManager.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import <Foundation/Foundation.h>

@interface VRVIUNotificationManager : NSObject

- (nullable instancetype)init;

- (void)addObserver:(nonnull id)observer
           selector:(nonnull SEL)aSelector
               name:(nullable NSString *)aName
             object:(nullable id)anObject;

- (void)removeAllObservers:(nonnull id)observer;

@end
