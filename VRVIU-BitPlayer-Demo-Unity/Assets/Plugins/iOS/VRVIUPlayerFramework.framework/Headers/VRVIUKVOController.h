/*
 * VRVIUKVOController.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 *
 */

#import <Foundation/Foundation.h>

@interface VRVIUKVOController : NSObject

- (id)initWithTarget:(NSObject *)target;

- (void)safelyAddObserver:(NSObject *)observer
               forKeyPath:(NSString *)keyPath
                  options:(NSKeyValueObservingOptions)options
                  context:(void *)context;
- (void)safelyRemoveObserver:(NSObject *)observer
                  forKeyPath:(NSString *)keyPath;

- (void)safelyRemoveAllObservers;

@end
