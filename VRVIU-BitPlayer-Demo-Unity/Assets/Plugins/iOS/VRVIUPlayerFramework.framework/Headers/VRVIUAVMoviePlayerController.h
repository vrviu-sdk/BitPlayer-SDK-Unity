/*
 * VRVIUAVMoviePlayerController.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix

*/

#import "VRVIUMediaPlayback.h"

@interface VRVIUAVMoviePlayerController : NSObject <VRVIUMediaPlayback>

- (id)initWithContentURL:(NSURL *)aUrl;
- (id)initWithContentURLString:(NSString *)aUrl;
+ (id)getInstance:(NSString *)aUrl;

@end
