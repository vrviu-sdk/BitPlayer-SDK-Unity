/*
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import <Foundation/Foundation.h>

@interface VRVIUMonitor : NSObject

- (instancetype)init;

@property(nonatomic) NSDictionary *mediaMeta;
@property(nonatomic) NSDictionary *videoMeta;
@property(nonatomic) NSDictionary *audioMeta;

@property(nonatomic, readonly) int64_t   duration;   // milliseconds
@property(nonatomic, readonly) int64_t   bitrate;    // bit / sec
@property(nonatomic, readonly) float     fps;        // frame / sec
@property(nonatomic, readonly) int       width;      // width
@property(nonatomic, readonly) int       height;     // height
@property(nonatomic, readonly) NSString *vcodec;     // video codec
@property(nonatomic, readonly) NSString *acodec;     // audio codec
@property(nonatomic, readonly) int       sampleRate;
@property(nonatomic, readonly) int64_t   channelLayout;

@property(nonatomic) NSString *vdecoder;

@property(nonatomic) int       tcpError;
@property(nonatomic) NSString *remoteIp;

@property(nonatomic) int       httpError;
@property(nonatomic) NSString *httpUrl;
@property(nonatomic) NSString *httpHost;
@property(nonatomic) int       httpCode;
@property(nonatomic) int64_t   httpOpenTick;
@property(nonatomic) int64_t   httpSeekTick;
@property(nonatomic) int       httpOpenCount;
@property(nonatomic) int       httpSeekCount;
@property(nonatomic) int64_t   lastHttpOpenDuration;
@property(nonatomic) int64_t   lastHttpSeekDuration;
@property(nonatomic) int64_t   filesize;

@property(nonatomic) int64_t   prepareStartTick;
@property(nonatomic) int64_t   prepareDuration;
@property(nonatomic) int64_t   firstVideoFrameLatency;
@property(nonatomic) int64_t   lastPrerollStartTick;
@property(nonatomic) int64_t   lastPrerollDuration;

// add by felix
@property(nonatomic) int64_t   playClicked;
@property(nonatomic) int64_t   httpBegin;//httpOpenTick
@property(nonatomic) int64_t   recvHttp;//lastHttpOpenDuration
@property(nonatomic) int64_t   firstKeyFrame;
@property(nonatomic) int64_t   renderFirstFrame;
//

@end
