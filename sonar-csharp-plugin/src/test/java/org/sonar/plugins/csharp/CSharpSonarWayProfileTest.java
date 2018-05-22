/*
 * SonarC#
 * Copyright (C) 2014-2018 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
package org.sonar.plugins.csharp;

import com.google.common.collect.Sets;
import com.sonar.plugins.security.api.CsRules;
import java.util.HashSet;
import org.junit.Test;
import org.sonar.api.rule.RuleKey;
import org.sonar.api.server.profile.BuiltInQualityProfilesDefinition;

import static org.assertj.core.api.Assertions.assertThat;

public class CSharpSonarWayProfileTest {

  @Test
  public void test() {
    CSharpSonarWayProfile profileDef = new CSharpSonarWayProfile();
    BuiltInQualityProfilesDefinition.Context context = new BuiltInQualityProfilesDefinition.Context();
    CsRules.ruleKeys = new HashSet<>();
    profileDef.define(context);
    BuiltInQualityProfilesDefinition.BuiltInQualityProfile profile = context.profile("cs", "Sonar way");
    assertThat(profile.language()).isEqualTo(CSharpPlugin.LANGUAGE_KEY);
    // FIXME : should be null when https://github.com/SonarSource/sonar-csharp/issues/1386 is fixed
    assertThat(profile.rule(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "S3649"))).isNotNull();
    CsRules.ruleKeys = Sets.newHashSet("S3649");
    assertThat(profile.rule(RuleKey.of(CSharpPlugin.REPOSITORY_KEY, "S3649"))).isNotNull();
  }

}
