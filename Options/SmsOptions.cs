﻿namespace CksysRecruitNew.Server.Options;

public class SmsOptions {
  public const string SectionKey = "SmsOptions";

  public string AccessKeyId { get; set; } = null!;

  public string AccessKeySecret { get; set; } = null!;

  public string Endpoint { get; set; } = null!;
}
