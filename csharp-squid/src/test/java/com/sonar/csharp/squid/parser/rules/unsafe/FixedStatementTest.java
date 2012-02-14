/*
 * Copyright (C) 2010 SonarSource SA
 * All rights reserved
 * mailto:contact AT sonarsource DOT com
 */
package com.sonar.csharp.squid.parser.rules.unsafe;

import static com.sonar.sslr.test.parser.ParserMatchers.*;
import static org.junit.Assert.*;

import java.nio.charset.Charset;

import org.junit.Before;
import org.junit.Test;

import com.sonar.csharp.squid.CSharpConfiguration;
import com.sonar.csharp.squid.api.CSharpGrammar;
import com.sonar.csharp.squid.parser.CSharpParser;
import com.sonar.sslr.impl.Parser;

public class FixedStatementTest {

  private final Parser<CSharpGrammar> p = CSharpParser.create(new CSharpConfiguration(Charset.forName("UTF-8")));
  private final CSharpGrammar g = p.getGrammar();

  @Before
  public void init() {
    p.setRootRule(g.fixedStatement);
  }

  @Test
  public void testOk() {
    g.pointerType.mock();
    g.fixedPointerDeclarator.mock();
    g.embeddedStatement.mock();
    assertThat(p, parse("fixed(pointerType fixedPointerDeclarator) embeddedStatement"));
    assertThat(p, parse("fixed(pointerType fixedPointerDeclarator, fixedPointerDeclarator, fixedPointerDeclarator) embeddedStatement"));
  }

  @Test
  public void testKo() {
    assertThat(p, notParse(""));
  }

  @Test
  public void testRealLife() throws Exception {
    assertThat(p, parse("fixed (int* p = stackalloc int[100], q = &y) {}"));
  }

}
