
#import <AdSupport/AdSupport.h>

#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL
const char * advertisingIdentifier()
{
	 NSString *ss = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
	 if (ss)
		 return MakeStringCopy( ss );
	return MakeStringCopy( @"" );
}